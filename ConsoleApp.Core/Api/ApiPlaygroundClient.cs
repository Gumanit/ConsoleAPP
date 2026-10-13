using System.Net.Http.Json;
using System.Text.Json;

namespace ConsoleApp.Core.Api;

/// <summary>
/// Демонстрационный клиент для ручной проверки работы с внешним REST API
/// (GET- и POST-запросы к jsonplaceholder).
/// </summary>
/// <param name="httpClient">Настроенный экземпляр <see cref="HttpClient"/>.</param>
public sealed class ApiPlaygroundClient(HttpClient httpClient)
{
    /// <summary>GET /todos — возвращает первые <paramref name="limit"/> задач в виде JSON.</summary>
    /// <param name="limit">Сколько записей вывести.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task<string> GetTodosAsync(int limit, CancellationToken cancellationToken = default)
    {
        var todos = await httpClient.GetFromJsonAsync(
            "todos",
            ApiTodoJsonContext.Default.ListApiTodo,
            cancellationToken) ?? [];

        var selected = todos.Take(limit).ToList();
        return JsonSerializer.Serialize(selected, ApiTodoJsonContext.Default.ListApiTodo);
    }

    /// <summary>GET /todos/{id} — возвращает одну задачу в виде JSON.</summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task<string> GetTodoByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var todo = await httpClient.GetFromJsonAsync(
            $"todos/{id}",
            ApiTodoJsonContext.Default.ApiTodo,
            cancellationToken);

        return todo is null
            ? "(данные не получены)"
            : JsonSerializer.Serialize(todo, ApiTodoJsonContext.Default.ApiTodo);
    }

    /// <summary>POST /todos — отправляет новую задачу и возвращает ответ сервера.</summary>
    /// <param name="title">Заголовок задачи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task<string> CreateTodoAsync(string title, CancellationToken cancellationToken = default)
    {
        var payload = new ApiTodo { UserId = 1, Title = title, Completed = false };

        using var response = await httpClient.PostAsJsonAsync(
            "todos",
            payload,
            ApiTodoJsonContext.Default.ApiTodo,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
