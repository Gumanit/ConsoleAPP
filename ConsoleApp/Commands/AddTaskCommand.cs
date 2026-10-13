using ConsoleApp.Core.Extensions;
using ConsoleApp.Core.Services;
using ConsoleApp.Menu;

namespace ConsoleApp.Commands;

/// <summary>Добавляет новую задачу.</summary>
internal sealed class AddTaskCommand(TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "2";

    /// <inheritdoc />
    public string Title => "Добавить задачу";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var title = ConsolePrompt.Ask("Название задачи");
        var description = ConsolePrompt.AskOptional("Описание");
        var priority = ConsolePrompt.AskPriority();
        var dueDate = ConsolePrompt.AskDate("Срок (например, 2026-12-31 или 2026-12-31 18:00)");
        var category = ConsolePrompt.AskOptional("Категория");
        var tags = ConsolePrompt.AskOptional("Теги (через запятую)");

        var task = await service.AddAsync(
            title,
            description,
            priority,
            dueDate,
            category,
            tags,
            cancellationToken);

        Console.WriteLine($"Задача добавлена: {task.Id}");
        Console.WriteLine($"  {task.Describe()}");
    }
}
