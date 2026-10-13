using ConsoleApp.Core.Repositories;
using ConsoleApp.Core.Services;
using ConsoleApp.Menu;

namespace ConsoleApp.Commands;

/// <summary>Показывает задачи с возможностью фильтрации.</summary>
internal sealed class ListTasksCommand(TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "1";

    /// <inheritdoc />
    public string Title => "Список задач (с фильтрами)";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var filter = new TaskFilter
        {
            Status = ConsolePrompt.AskStatus(),
            Priority = ConsolePrompt.AskBool("Фильтровать по приоритету") ? ConsolePrompt.AskPriority() : null,
            Category = ConsolePrompt.AskCategory(),
            Tag = ConsolePrompt.AskTag(),
            OnlyOverdue = ConsolePrompt.AskBool("Только просроченные"),
        };

        var tasks = await service.ListAsync(filter, cancellationToken);
        ConsoleOutput.PrintTasks(tasks);
    }
}
