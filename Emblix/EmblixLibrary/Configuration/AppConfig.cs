using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmblixLibrary.Configuration;

public sealed class AppConfig
{
    private static readonly object _lock = new();
    private static AppConfig? _instance;
    
    public static AppConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = LoadConfiguration();
                    }
                }
            }
            return _instance;
        }
    }
    
    public string Domain { get; }
    public uint Port { get; }
    public string StaticDirectoryPath { get; }
    public string TemplatesPath { get; }  // Добавьте это свойство
    
    private AppConfig()
    {
        Domain = "localhost";
        Port = 4545;
        StaticDirectoryPath = @"public/";
        TemplatesPath = @"Templates/";    // И здесь значение по умолчанию
    }

    [JsonConstructor]
    public AppConfig(string domain, uint port, string staticDirectoryPath, string templatesPath) // Добавьте параметр
    {
        Domain = domain ?? "localhost";
        Port = port > 0 ? port : 4545;
        StaticDirectoryPath = staticDirectoryPath ?? @"public/";
        TemplatesPath = templatesPath ?? @"Templates/";    // И здесь присвоение
    }

    /// <summary>
    /// Загрузка конфигурации из файла config.json.
    /// Если файл отсутствует или некорректен, используются значения по умолчанию.
    /// </summary>
    /// <returns>Экземпляр <see cref="AppConfig"/>.</returns>
    private static AppConfig LoadConfiguration()
    {
        const string configPath = "config.json";

        if (File.Exists(configPath))
        {
            try
            {
                var fileConfig = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<AppConfig>(fileConfig) ?? new AppConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки конфигурации из файла: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Файл конфигурации '{configPath}' не найден. Используются значения по умолчанию.");
        }

        return new AppConfig();
    }
}