using System.Net;
using EmblixLibrary.Core;
using EmblixLibrary.Handlers;

namespace EmblixLibrary;

public sealed class EmblixServer
{
    // Слушатель HTTP-запросов
    private readonly HttpListener _listener;
    
    // Обработчик статических файлов
    private readonly StaticFilesHandler _staticFilesHandler;

    // Обработчик конечных точек
    private readonly EndpointsHandler _endpointsHandler;

    /// <summary>
    /// Конструктор сервера.
    /// Инициализирует слушатель и добавляет префиксы.
    /// </summary>
    /// <param name="prefixes">Список URL-префиксов для прослушивания.</param>
    public EmblixServer(string[] prefixes)
    {
        _listener = new HttpListener();

        // Добавление всех указанных префиксов
        foreach (string prefix in prefixes)
        {
            _listener.Prefixes.Add(prefix); // Префикс для прослушивания запросов
        }
        
        _staticFilesHandler = new StaticFilesHandler(); // Инициализация обработчика статических файлов
        _endpointsHandler = new EndpointsHandler();     // Инициализация обработчика конечных точек
    }

    /// <summary>
    /// Запускает сервер и начинает обработку запросов.
    /// </summary>
    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("Сервер запущен и ожидает запросов");

        while (_listener.IsListening)
        {
            // Получение контекста запроса
            var context = await _listener.GetContextAsync();

            // Преобразование в контекст HTTP-запроса
            var httpRequestContext = new HttpRequestContext(context);

            // Обработка запроса
            await ProcessRequestAsync(httpRequestContext);
        }
    }
    
    /// <summary>
    /// Обрабатывает HTTP-запрос с использованием цепочки обработчиков.
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса.</param>
    private async Task ProcessRequestAsync(HttpRequestContext context)
    {
        // Редирект с корневого URL на /emblix
        if (context.Request.Url.LocalPath == "/")
        {
            context.Response.Redirect("/emblix");
            context.Response.Close();
            return;
        }

        // Существующий код обработки...
        _staticFilesHandler.Successor = _endpointsHandler;
        _staticFilesHandler.HandleRequest(context);
    }  
    
    /// <summary>
    /// Останавливает сервер и завершает обработку запросов.
    /// </summary>
    public void Stop()
    {
        _listener.Stop();
        Console.WriteLine("Сервер остановлен");
    }
}
