using System.Net;

namespace EmblixLibrary.Core;

public class HttpRequestContext
{
    public HttpListenerRequest Request { get; set; }
    public HttpListenerResponse Response { get; set; }
    public string RequestBody { get; set; } // Добавляем свойство для тела запроса

    public HttpRequestContext(HttpListenerContext context)
    {
        Request = context.Request;
        Response = context.Response;
        
        // Читаем тело запроса для POST-запросов
        if (Request.HttpMethod == "POST" && Request.HasEntityBody)
        {
            using (var reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            {
                RequestBody = reader.ReadToEnd();
            }
        }
    }
}