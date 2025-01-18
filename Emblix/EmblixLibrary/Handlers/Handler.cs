using EmblixLibrary.Core;

namespace EmblixLibrary.Handlers;

public abstract class Handler
{
    /// <summary>
    /// Следующий обработчик в цепочке.
    /// Если текущий обработчик не может обработать запрос, он передает его следующему.
    /// </summary>
    public Handler Successor { get; set; }

    /// <summary>
    /// Метод для обработки HTTP-запроса.
    /// Должен быть реализован в дочерних классах.
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса.</param>
    public abstract void HandleRequest(HttpRequestContext context);
}