using ConsoleApp.Core.Services;
using ConsoleApp.Menu;

namespace ConsoleApp.Commands;

/// <summary>Экспортирует задачи в JSON-файл.</summary>
internal sealed class ExportJsonCommand(TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "9";

    /// <inheritdoc />
    public string Title => "Экспорт задач в JSON";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var path = ConsolePrompt.Ask("Путь к файлу", "tasks-export.json");
        await service.ExportJsonAsync(path, cancellationToken);
        Console.WriteLine($"Данные сохранены в {Path.GetFullPath(path)}.");
    }
}
