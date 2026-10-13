using ConsoleApp.Core.Services;
using ConsoleApp.Menu;

namespace ConsoleApp.Commands;

/// <summary>Удаляет задачу.</summary>
internal sealed class DeleteTaskCommand(TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "5";

    /// <inheritdoc />
    public string Title => "Удалить задачу";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var id = ConsolePrompt.AskGuid("Идентификатор задачи");

        if (!ConsolePrompt.AskBool("Подтвердите удаление"))
        {
            Console.WriteLine("Удаление отменено.");
            return;
        }

        var deleted = await service.DeleteAsync(id, cancellationToken);

        Console.WriteLine(deleted
            ? "Задача удалена."
            : "Задача с таким идентификатором не найдена.");
    }
}
