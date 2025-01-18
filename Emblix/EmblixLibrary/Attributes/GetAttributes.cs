namespace EmblixLibrary.Attributes;

public sealed class GetAttribute : Attribute
{
    /// <summary>
    /// Роут (маршрут), по которому доступен метод для обработки GET-запроса.
    /// </summary>
    public string Route { get; }

    /// <summary>
    /// Инициализирует новый экземпляр атрибута с указанным маршрутом.
    /// </summary>
    /// <param name="route">Маршрут, ассоциированный с методом.</param>
    public GetAttribute(string route)
    {
        Route = route;
    }
}