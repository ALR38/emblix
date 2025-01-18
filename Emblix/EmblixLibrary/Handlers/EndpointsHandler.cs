using System.Reflection;
using System.Web;
using EmblixLibrary.Attributes;
using EmblixLibrary.Core;
using EmblixLibrary.Response;

namespace EmblixLibrary.Handlers;

public class EndpointsHandler : Handler
{
    private readonly Dictionary<string, List<(HttpMethod method, MethodInfo handler, Type endpointType)>> _routes = new();

    public EndpointsHandler()
    {
        RegisterEndpointsFromAssemblies(new[] { Assembly.GetEntryAssembly() });
    }

    public override void HandleRequest(HttpRequestContext context)
    {
        // Пробуем сначала обработать как статический файл
        if (Successor != null && context.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            // Пытаемся обработать запрос через StaticFilesHandler
            Successor.HandleRequest(context);
        }
        
        var url = context.Request.Url.LocalPath.Trim('/');
        var methodType = context.Request.HttpMethod;

        if (_routes.ContainsKey(url))
        {
            var route = _routes[url].FirstOrDefault(r => r.method.ToString().Equals(methodType, StringComparison.InvariantCultureIgnoreCase));
            if (route.handler != null)
            {
                var endpointInstance = Activator.CreateInstance(route.endpointType) as BaseEndpoint;
                if (endpointInstance != null)
                {
                    endpointInstance.SetContext(context);

                    var parameters = ExtractParameters(context, route.handler);
                    var result = route.handler.Invoke(endpointInstance, parameters) as IResponseResult;
                    result.Execute(context.Response);
                }
            }
        }
        else if (Successor != null)
        {
            Successor.HandleRequest(context);
        }
        
        context.Response.Close();
    }

    private void RegisterEndpointsFromAssemblies(Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var endpointsTypes = assembly.GetTypes()
                .Where(t => typeof(BaseEndpoint).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var endpointType in endpointsTypes)
            {
                var methods = endpointType.GetMethods();
                foreach (var method in methods)
                {
                    var getAttribute = method.GetCustomAttribute<GetAttribute>();
                    if (getAttribute != null)
                    {
                        RegisterRoute(getAttribute.Route, HttpMethod.Get, method, endpointType);
                    }

                    var postAttribute = method.GetCustomAttribute<PostAttribute>();
                    if (postAttribute != null)
                    {
                        RegisterRoute(postAttribute.Route, HttpMethod.Post, method, endpointType);
                    }
                }
            }
        }
    }

    private void RegisterRoute(string route, HttpMethod method, MethodInfo handler, Type endpointType)
    {
        if (_routes.ContainsKey(route) && _routes[route].Any(r => r.method == method))
        {
            Console.WriteLine($"Ошибка: Маршрут '{route}' с методом '{method}' уже зарегистрирован.");
            throw new InvalidOperationException($"Маршрут '{route}' с методом '{method}' уже существует.");
        }

        if (!_routes.ContainsKey(route))
        {
            _routes[route] = new List<(HttpMethod, MethodInfo, Type)>();
        }

        _routes[route].Add((method, handler, endpointType));
    }

    private object[] ExtractParameters(HttpRequestContext context, MethodInfo handler)
    {
        var parameters = handler.GetParameters();
        var result = new List<object>();
        
        using var reader = new StreamReader(context.Request.InputStream);
        string body = reader.ReadToEnd();
        var data = HttpUtility.ParseQueryString(body);

        if (context.Request.HttpMethod == "GET" || context.Request.HttpMethod == "POST")
        {
            foreach (var parameter in parameters)
            {
                if (context.Request.HttpMethod == "GET")
                {
                    result.Add(Convert.ChangeType(context.Request.QueryString[parameter.Name], parameter.ParameterType));
                }
                else if (context.Request.HttpMethod == "POST")
                {
                    result.Add(Convert.ChangeType(data[parameter.Name], parameter.ParameterType));
                }
            }
        }
        else
        {
            var urlSegments = context.Request.Url.Segments
                .Skip(2) // Пропуск первых двух сегментов
                .Select(s => s.Replace("/", ""))
                .ToArray();

            for (int i = 0; i < parameters.Length; i++)
            {
                result.Add(Convert.ChangeType(urlSegments[i], parameters[i].ParameterType));
            }
        }

        return result.ToArray();
    }
}