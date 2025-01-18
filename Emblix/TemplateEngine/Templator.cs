using System.Text;
using System.Text.RegularExpressions;

namespace Emblix.Core.Templates;

public interface ITemplator
{
    string GetHtmlByTemplate(string template, string name);
    string GetHtmlByTemplate<T>(string template, T obj);
}

public class Templator : ITemplator
{
    public string GetHtmlByTemplate(string template, string name)
    {
        return template.Replace("{{name}}", name);
    }

    public string GetHtmlByTemplate<T>(string template, T obj)
    {
        if (string.IsNullOrEmpty(template) || obj == null)
            return template;

        // Обработка циклов
        template = ProcessLoops(template, obj);
        
        // Обработка условий eq
        template = ProcessEqConditions(template, obj);
        
        // Обработка простых условий
        template = ProcessConditions(template, obj);
        
        // Обработка вложенных объектов
        template = ProcessNestedObjects(template, obj);
        
        // Обработка простых переменных
        template = ReplaceVariables(template, obj);

        return template;
    }

    private string ReplaceVariables<T>(string template, T obj)
    {
        var result = template;
        var props = obj.GetType().GetProperties();

        foreach (var prop in props)
        {
            var key = $"{{{{{prop.Name}}}}}";
            var val = prop.GetValue(obj);
            
            if (val is DateTime dt)
            {
                val = dt.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));
            }
            else if (val is IEnumerable<object> collection && !(val is string))
            {
                continue; // Пропускаем коллекции, они обрабатываются в ProcessLoops
            }
            else if (val is string strVal)
            {
                val = string.IsNullOrEmpty(strVal) ? string.Empty : strVal;
            }
            else if (val == null)
            {
                val = string.Empty;
            }

            result = result.Replace(key, val.ToString());
        }

        return result;
    }

    private string ProcessEqConditions<T>(string template, T obj)
    {
        var regex = new Regex(@"\{\{#if \(eq (.*?) (.*?)\)\}\}(.*?)(?:\{\{else\}\}(.*?))?\{\{/if\}\}", RegexOptions.Singleline);
        return regex.Replace(template, match =>
        {
            var prop1Name = match.Groups[1].Value.Trim('\'', '"');
            var prop2Name = match.Groups[2].Value.Trim('\'', '"');
            var contentIf = match.Groups[3].Value;
            var contentElse = match.Groups[4].Success ? match.Groups[4].Value : string.Empty;

            var prop1 = obj.GetType().GetProperty(prop1Name);
            var value1 = prop1?.GetValue(obj)?.ToString() ?? prop1Name;

            var prop2 = obj.GetType().GetProperty(prop2Name);
            var value2 = prop2?.GetValue(obj)?.ToString() ?? prop2Name;

            if (string.IsNullOrEmpty(value1) && string.IsNullOrEmpty(value2))
                return contentElse;

            return value1 == value2 ? contentIf : contentElse;
        });
    }

    private string ProcessLoops<T>(string template, T obj)
    {
        var regex = new Regex(@"\{\{#each (.*?)\}\}(.*?)\{\{/each\}\}", RegexOptions.Singleline);
        return regex.Replace(template, match =>
        {
            var collectionName = match.Groups[1].Value;
            var itemTemplate = match.Groups[2].Value;

            var prop = obj.GetType().GetProperty(collectionName);
            if (prop != null)
            {
                var collection = prop.GetValue(obj) as System.Collections.IEnumerable;
                if (collection != null)
                {
                    var result = new StringBuilder();
                    var items = collection.Cast<object>().ToList();
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        var processedItem = itemTemplate;
                        processedItem = ProcessConditions(processedItem, item);
                        processedItem = ReplaceVariables(processedItem, item);

                        // Обработка {{#unless @last}}
                        processedItem = processedItem.Replace("{{#unless @last}}", i < items.Count - 1 ? "" : "<!--");
                        processedItem = processedItem.Replace("{{/unless}}", i < items.Count - 1 ? "" : "-->");

                        result.Append(processedItem);
                    }
                    return result.ToString();
                }
            }
            return string.Empty;  // Если коллекция не найдена, возвращаем пустую строку
        });
    }

    private string ProcessConditions<T>(string template, T obj)
    {
        var regex = new Regex(@"\{\{#if (.*?)\}\}(.*?)(?:\{\{else\}\}(.*?))?\{\{/if\}\}", RegexOptions.Singleline);
        return regex.Replace(template, match =>
        {
            var propertyName = match.Groups[1].Value;
            var contentIf = match.Groups[2].Value;
            var contentElse = match.Groups[3].Success ? match.Groups[3].Value : string.Empty;

            var prop = obj.GetType().GetProperty(propertyName);
            if (prop != null)
            {
                var value = prop.GetValue(obj);
                if (value is bool boolVal)
                {
                    return boolVal ? contentIf : contentElse;
                }
                if (value is string strVal)
                {
                    return !string.IsNullOrEmpty(strVal) ? contentIf : contentElse;
                }
                return value != null ? contentIf : contentElse;
            }
            return string.Empty;  // Если свойство не найдено, возвращаем пустую строку
        });
    }

    private string ProcessNestedTemplate(string template, object item, object parentContext)
    {
        // Обработка обращений к родительскому контексту (../Property)
        var parentRegex = new Regex(@"\{\{\.\./([^}]+)\}\}");
        template = parentRegex.Replace(template, match =>
        {
            var propertyName = match.Groups[1].Value;
            var prop = parentContext.GetType().GetProperty(propertyName);
            return prop?.GetValue(parentContext)?.ToString() ?? string.Empty;
        });

        // Обработка обычных свойств текущего элемента
        return RenderItem(template, item);
    }

    private string RenderItem(string template, object item)
    {
        var result = template;
        var props = item.GetType().GetProperties();

        foreach (var prop in props)
        {
            var key = $"{{{{{prop.Name}}}}}";
            var val = prop.GetValue(item);

            if (val is IEnumerable<string> strList && !(val is string))
            {
                val = string.Join(", ", strList);
            }
            else if (val is DateTime dt)
            {
                val = dt.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));
            }
            else if (val is IEnumerable<object> collection && !(val is string))
            {
                continue; // Пропускаем коллекции, они должны обрабатываться через #each
            }
            else if (val is string strVal)
            {
                val = string.IsNullOrEmpty(strVal) ? string.Empty : strVal;
            }
            else if (val == null)
            {
                val = string.Empty;
            }

            result = result.Replace(key, val.ToString());
        }

        return result;
    }

    private string ProcessNestedObjects<T>(string template, T obj)
    {
        var regex = new Regex(@"\{\{([\w\.]+)\}\}");
        return regex.Replace(template, match =>
        {
            var path = match.Groups[1].Value.Split('.');
            object value = obj;
            
            foreach (var prop in path)
            {
                if (value == null) break;
                var propInfo = value.GetType().GetProperty(prop);
                value = propInfo?.GetValue(value);
            }
            
            if (value is IEnumerable<object> && !(value is string))
                return string.Empty;
                
            return value?.ToString() ?? string.Empty;
        });
    }

    private string ProcessNestedProperties(string template, object obj)
    {
        var regex = new Regex(@"\{\{([\w\.]+)\}\}");
        return regex.Replace(template, match =>
        {
            var propertyPath = match.Groups[1].Value.Split('.');
            var value = obj;

            foreach (var prop in propertyPath)
            {
                if (value == null) break;
                var propInfo = value.GetType().GetProperty(prop);
                value = propInfo?.GetValue(value);
            }

            if (value is string strVal)
            {
                return string.IsNullOrEmpty(strVal) ? string.Empty : strVal;
            }

            return value?.ToString() ?? string.Empty;
        });
    }
}