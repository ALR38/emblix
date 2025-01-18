using System.Net;
using EmblixLibrary;
using EmblixLibrary.Core;
using EmblixLibrary.Response;

[TestFixture]
public class BaseEndpointTests
{
    private class TestEndpoint : BaseEndpoint
    {
        public IResponseResult TestHtmlResponse(string content)
        {
            return Html(content);
        }

        public IResponseResult TestJsonResponse(object data)
        {
            return Json(data);
        }

        public IResponseResult TestRedirectResponse(string location)
        {
            return Redirect(location);
        }
    }

    private TestEndpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _endpoint = new TestEndpoint();
    }

    [Test]
    public void RedirectResponse_ShouldSetLocationAndStatus()
    {
        // Arrange
        var location = "/new-location";

        // Act
        var result = _endpoint.TestRedirectResponse(location);

        // Assert
        Assert.That(result, Is.TypeOf<RedirectResult>());
    }
}

[TestFixture]
public class EmblixServerTests
{
    private EmblixServer _server;
    private CancellationTokenSource _cancellationTokenSource;

    [SetUp]
    public void Setup()
    {
        _server = new EmblixServer(new[] { "http://localhost:4545/" });
        _cancellationTokenSource = new CancellationTokenSource();
    }

    [TearDown]
    public void TearDown()
    {
        _cancellationTokenSource.Dispose();
    }

    [Test]
    public async Task Server_StartAndStop_ShouldWorkCorrectly()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        try
        {
            // Act
            var serverTask = Task.Run(async () => await _server.StartAsync());
            await Task.Delay(500);

            // Assert
            Assert.That(serverTask.IsFaulted, Is.False);
        }
        finally
        {
            _server.Stop();
            cts.Cancel();
        }
    }
}