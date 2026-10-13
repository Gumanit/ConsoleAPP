using ConsoleApp.Core.Services;
using ConsoleApp.Menu;

namespace ConsoleApp.Commands;

/// <summary>Импортирует задачи из JSON-файла.</summary>
internal sealed class ImportJsonCommand(TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "10";

    /// <inheritdoc />
    public string Title => "Импорт задач из JSON";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var path = ConsolePrompt.Ask("Путь к файлу", "tasks-export.json");

        if (!File.Exists(path))
        {
            Console.WriteLine($"Файл не найден: {Path.GetFullPath(path)}");
            return;
        }

        var imported = await service.ImportJsonAsync(path, cancellationToken);
        Console.WriteLine($"Импортировано задач: {imported}.");
    }
}
