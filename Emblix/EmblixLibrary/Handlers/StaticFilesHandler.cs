using System.Net;
using EmblixLibrary.Configuration;
using EmblixLibrary.Core;

namespace EmblixLibrary.Handlers;

public class StaticFilesHandler : Handler
{
    // Путь к директории со статическими файлами
    private readonly string _staticDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), AppConfig.Instance.StaticDirectoryPath);

    /// <summary>
    /// Обрабатывает запрос на получение статического файла.
    /// Если файл найден, возвращает его содержимое.
    /// Если нет, передает запрос следующему обработчику.
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса.</param>
    public override void HandleRequest(HttpRequestContext context)
    {
        // Проверка, является ли запрос GET-запросом
        bool isGet = context.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase);

        // Проверка, имеет ли запрос на файл расширение
        string[] arr = context.Request.Url?.AbsolutePath.Split('.');
        bool isFile = arr.Length >= 2;

        if (isGet && isFile)
        {
            // Формирование пути к файлу
            string relativePath = context.Request.Url?.AbsolutePath.TrimStart('/') ?? "emblix.html";
            string filePath = Path.Combine(_staticDirectoryPath, relativePath);

            try
            {
                // Проверка существования файла
                if (!File.Exists(filePath))
                {
                    // Если файл не найден, вернуть 404
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.OutputStream.Close();
                    return;
                }

                // Чтение и отправка содержимого файла в ответ
                byte[] responseFile = File.ReadAllBytes(filePath);
                context.Response.ContentType = GetContentType(Path.GetExtension(filePath)); 
                context.Response.ContentLength64 = responseFile.Length;
                context.Response.OutputStream.Write(responseFile, 0, responseFile.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                // Логирование ошибки и возврат 500
                Console.WriteLine($"Ошибка при отправке статического файла: {ex.Message}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.OutputStream.Close();
            }
        }
        // Передача запроса следующему обработчику, если файл не был найден
        else if (Successor != null)
        {
            Successor.HandleRequest(context);
        }
    }

    /// <summary>
    /// Определяет тип файла по его расширению.
    /// </summary>
    /// <param name="extension">Расширение файла.</param>
    /// <returns>Тип файла.</returns>
    /// <exception cref="ArgumentNullException">Бросается, если расширение null.</exception>
    private string GetContentType(string extension)
    {
        if (extension == null)
        {
            throw new ArgumentNullException(nameof(extension), "Extension cannot be null.");
        }

        // Определение типа содержимого по расширению
        return extension.ToLower() switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream", // Тип по умолчанию
        };
    }
}