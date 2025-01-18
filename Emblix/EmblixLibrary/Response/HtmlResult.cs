using System.Net;
using System.Text;

namespace EmblixLibrary.Response;

internal class HtmlResult : IResponseResult
{
    /// <summary>
    /// The HTML content to be returned in the response
    /// </summary>
    private readonly string _html;

    /// <summary>
    /// Initializes a new instance of the HtmlResult class with the specified HTML content
    /// </summary>
    /// <param name="html">The HTML content to be returned.</param>
    public HtmlResult(string html)
    {
        _html = html;
    }

    /// <summary>
    /// Executes the HTTP response by writing the HTML content to the response stream.
    /// </summary>
    /// <param name="response">The HttpListenerResponse object to which the HTML content will be written.</param>
    public void Execute(HttpListenerResponse response) // сюда приходит context.Response
    {
        // responce.Headers.Add("Content-Type", "text/html");
        byte[] buffer = Encoding.UTF8.GetBytes(_html);
        response.ContentLength64 = buffer.Length;
        response.ContentType = "text/html";
        using Stream output = response.OutputStream;
        output.WriteAsync(buffer);
        output.FlushAsync();
    }
    
}