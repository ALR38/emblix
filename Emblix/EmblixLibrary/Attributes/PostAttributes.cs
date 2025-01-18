namespace EmblixLibrary.Attributes;

public sealed class PostAttribute : Attribute
{
    /// <summary>
    /// Роут (маршрут), по которому доступен метод для обработки POST-запроса.
    /// </summary>
    public string Route { get; }

    /// <summary>
    /// Инициализирует новый экземпляр атрибута с указанным маршрутом.
    /// </summary>
    /// <param name="route">Маршрут, ассоциированный с методом.</param>
    public PostAttribute(string route)
    {
        Route = route;
    }
}