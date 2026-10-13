namespace ConsoleApp.Core.Api;

/// <summary>
/// Клиент внешнего REST API со списком задач.
/// </summary>
public interface ITodoApiClient
{
    /// <summary>Возвращает не более <paramref name="limit"/> задач из внешнего API.</summary>
    /// <param name="limit">Максимальное количество задач.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<ApiTodo>> GetTodosAsync(int limit, CancellationToken cancellationToken = default);
}
