using System.Net;
using System.Text;
using System.Text.Json;

namespace EmblixLibrary.Response;

internal class JsonResult : IResponseResult
{
    private readonly object _data;

    public JsonResult(object data)
    {
        _data = data;
    }

    public void Execute(HttpListenerResponse context)
    {
        var json = JsonSerializer.Serialize(_data);

        byte[] buffer = Encoding.UTF8.GetBytes(json);
        context.Headers.Add("Content-Type", "application/json");
        // получаем поток ответа и пишем в него ответ
        context.ContentLength64 = buffer.Length;
        using Stream output = context.OutputStream;

        // отправляем данные
        output.Write(buffer);
        output.Flush();
    }
}