using System.Net;

namespace EmblixLibrary.Response;

public class RedirectResult: IResponseResult
{
    private readonly string _location;
    public RedirectResult(string location)
    {
        _location = location;
    }
 
    public void Execute(HttpListenerResponse context)
    {
        context.Redirect(_location);
        context.StatusCode = 302;
        context.OutputStream.Close();
    }

}