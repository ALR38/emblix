using System.Net;

namespace EmblixLibrary.Response;

public interface IResponseResult
{
    void Execute(HttpListenerResponse response);
}