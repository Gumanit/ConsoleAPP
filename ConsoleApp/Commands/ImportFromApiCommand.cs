using ConsoleApp.Core.Api;
using ConsoleApp.Core.Domain;
using ConsoleApp.Core.Services;
using ConsoleApp.Core.Services.Dto;
using ConsoleApp.Menu;

namespace ConsoleApp.Commands;

/// <summary>Загружает задачи из внешнего REST API (с offline-фолбэком).</summary>
internal sealed class ImportFromApiCommand(ITodoApiClient apiClient, TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "8";

    /// <inheritdoc />
    public string Title => "Импорт задач из внешнего API";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var limit = ConsolePrompt.AskInt("Сколько задач загрузить", 5);

        IReadOnlyList<ApiTodo> todos;
        var offline = false;

        try
        {
            todos = await apiClient.GetTodosAsync(limit, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            Console.WriteLine($"[API] Сервис недоступен ({exception.Message}). Использую offline-данные.");
            todos = OfflineTodoFallback.CreateDefault();
            offline = true;
        }

        var payload = todos
            .Take(limit)
            .Select(todo => new TaskDto
            {
                Title = todo.Title,
                Description = $"Импортировано из jsonplaceholder (id={todo.Id})",
                Priority = todo.Completed ? nameof(Priority.Low) : nameof(Priority.Medium),
                Status = todo.Completed ? nameof(TodoStatus.Done) : nameof(TodoStatus.New),
                Category = "Внешний API",
                Tags = ["jsonplaceholder"],
            })
            .ToList();

        var imported = await service.ImportAsync(payload, cancellationToken);
        var suffix = offline ? " (offline-режим)" : string.Empty;

        Console.WriteLine($"Импортировано задач: {imported}{suffix}.");
    }
}
