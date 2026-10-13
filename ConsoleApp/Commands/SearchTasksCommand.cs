using ConsoleApp.Core.Services;
using ConsoleApp.Menu;

namespace ConsoleApp.Commands;

/// <summary>Ищет задачи по названию и описанию.</summary>
internal sealed class SearchTasksCommand(TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "3";

    /// <inheritdoc />
    public string Title => "Поиск задач по тексту";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var text = ConsolePrompt.Ask("Строка поиска");
        var tasks = await service.SearchAsync(text, cancellationToken);
        ConsoleOutput.PrintTasks(tasks);
    }
}
