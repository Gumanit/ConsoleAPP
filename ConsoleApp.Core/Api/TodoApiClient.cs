using System.Net.Http.Json;

namespace ConsoleApp.Core.Api;

/// <summary>
/// HTTP-клиент публичного API jsonplaceholder для демонстрации работы с внешними сервисами.
/// </summary>
/// <param name="httpClient">Настроенный экземпляр <see cref="HttpClient"/>.</param>
public sealed class TodoApiClient(HttpClient httpClient) : ITodoApiClient
{
    private const string TodosEndpoint = "todos";

    /// <inheritdoc />
    public async Task<IReadOnlyList<ApiTodo>> GetTodosAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(limit);

        var todos = await httpClient.GetFromJsonAsync(
            TodosEndpoint,
            ApiTodoJsonContext.Default.ListApiTodo,
            cancellationToken);

        return todos is null
            ? []
            : todos.Take(limit).ToList();
    }
}
