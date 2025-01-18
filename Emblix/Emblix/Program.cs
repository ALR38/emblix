using EmblixLibrary;
using EmblixLibrary.Configuration;

namespace Emblix;

public class Program
{
    static async Task Main()
    {
        // Формирование префиксов на основе конфигурации приложения
        var prefixes = new[] { $"http://{AppConfig.Instance.Domain}:{AppConfig.Instance.Port}/" };
        
        // Создание HTTP-сервера с указанными префиксами
        var server = new EmblixServer(prefixes);

        // Асинхронный запуск сервера
        await server.StartAsync();
    }
}