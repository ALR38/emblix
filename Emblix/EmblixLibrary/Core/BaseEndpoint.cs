using EmblixLibrary.Configuration;
using EmblixLibrary.Response;
using EmblixLibrary.Session;

namespace EmblixLibrary.Core;

public class BaseEndpoint
{
    protected HttpRequestContext Context { get; private set; }

    public void SetContext(HttpRequestContext context)
    {
        Context = context;
        Console.WriteLine($"\n=== Request Context Set ===");
        Console.WriteLine($"Method: {context.Request.HttpMethod}");
        Console.WriteLine($"URL: {context.Request.Url}");
        Console.WriteLine($"Query String: {context.Request.QueryString}");
        
        if (context.Request.HttpMethod == "POST")
        {
            Console.WriteLine($"Content Type: {context.Request.ContentType}");
            Console.WriteLine($"Content Length: {context.Request.ContentLength64}");
            Console.WriteLine($"Request Body: {context.RequestBody}");
        }
    }
    
    protected string GetTemplate(string path)
    {
        Console.WriteLine($"\n=== Loading Template ===");
        Console.WriteLine($"Requested template path: {path}");
        
        try
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            var fullPath = Path.Combine(basePath, path);
        
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
            Console.WriteLine($"Looking for template at: {fullPath}");
        
            if (!Directory.Exists(basePath))
            {
                Console.WriteLine($"Templates directory does not exist: {basePath}");
                return string.Empty;
            }
        
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Template file not found: {fullPath}");
                var alternativePaths = new[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(), "Templates", path),
                    Path.Combine(Directory.GetCurrentDirectory(), path),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)
                };

                foreach (var altPath in alternativePaths)
                {
                    Console.WriteLine($"Trying alternative path: {altPath}");
                    if (File.Exists(altPath))
                    {
                        Console.WriteLine($"Found template at alternative path: {altPath}");
                        var content = File.ReadAllText(altPath);
                        Console.WriteLine($"Template loaded, length: {content.Length}");
                        return content;
                    }
                }
                
                return string.Empty;
            }
        
            var templateContent = File.ReadAllText(fullPath);
            Console.WriteLine($"Template loaded successfully, length: {templateContent.Length}");
            return templateContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading template: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return string.Empty;
        }
    }
    
    protected IResponseResult Html(string responseText)
    {
        Console.WriteLine($"\n=== Creating HTML Response ===");
        Console.WriteLine($"Response length: {responseText?.Length ?? 0}");
        return new HtmlResult(responseText);
    }

    protected IResponseResult Json(object data)
    {
        Console.WriteLine($"\n=== Creating JSON Response ===");
        Console.WriteLine($"Data type: {data?.GetType().Name ?? "null"}");
        return new JsonResult(data);
    }
    
    protected IResponseResult Redirect(string location)
    {
        Console.WriteLine($"\n=== Creating Redirect Response ===");
        Console.WriteLine($"Redirect location: {location}");
        return new RedirectResult(location);
    }
    
    protected Dictionary<string, string> ParseFormData(string requestBody)
    {
        var formData = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(requestBody)) return formData;

        var pairs = requestBody.Split('&');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                var key = Uri.UnescapeDataString(keyValue[0]);
                var value = Uri.UnescapeDataString(keyValue[1]);
                formData[key] = value;
            }
        }
        return formData;
    }
    
    public bool IsAuthorized(HttpRequestContext context)
    {
        Console.WriteLine($"\n=== Checking Authorization ===");
        var cookies = context.Request.Cookies;
        var sessionCookie = cookies["session-token"];
        
        if (sessionCookie != null)
        {
            var isValid = SessionStorage.ValidateToken(sessionCookie.Value);
            Console.WriteLine($"Session token found: {sessionCookie.Value}");
            Console.WriteLine($"Token is valid: {isValid}");
            return isValid;
        }
         
        Console.WriteLine("No session token found");
        return false;
    }
}