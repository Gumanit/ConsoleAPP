using ConsoleApp.Core.Api;

namespace ConsoleApp.Commands;

/// <summary>
/// Проверка работы с внешним REST API: выполняет GET-, GET-by-id- и POST-запросы
/// и печатает ответы сервера. Заменяет ручной HTTP-клиент, которого нет в IDE.
/// </summary>
internal sealed class ApiCheckCommand(ApiPlaygroundClient client) : ICommand
{
    /// <inheritdoc />
    public string Key => "13";

    /// <inheritdoc />
    public string Title => "Проверка API (GET/POST)";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine("=== GET /todos (первые 5) ===");
            Console.WriteLine(await client.GetTodosAsync(5, cancellationToken));

            Console.WriteLine();
            Console.WriteLine("=== GET /todos/1 ===");
            Console.WriteLine(await client.GetTodoByIdAsync(1, cancellationToken));

            Console.WriteLine();
            Console.WriteLine("=== POST /todos ===");
            Console.WriteLine(await client.CreateTodoAsync("Новая задача из OpenIDE", cancellationToken));
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            Console.WriteLine($"[API] Сервис недоступен: {exception.Message}");
            Console.WriteLine("Проверьте подключение к интернету и повторите.");
        }
    }
}
