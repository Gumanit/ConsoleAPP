using ConsoleApp.Core.Services;
using ConsoleApp.Menu;

namespace ConsoleApp.Commands;

/// <summary>Помечает задачу выполненной.</summary>
internal sealed class CompleteTaskCommand(TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "4";

    /// <inheritdoc />
    public string Title => "Отметить задачу выполненной";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var id = ConsolePrompt.AskGuid("Идентификатор задачи");
        var completed = await service.CompleteAsync(id, cancellationToken);

        Console.WriteLine(completed
            ? "Задача отмечена выполненной."
            : "Задача с таким идентификатором не найдена.");
    }
}
