using ConsoleApp.Core.Repositories;

namespace ConsoleApp.Commands;

/// <summary>Выводит список категорий и число задач в каждой.</summary>
internal sealed class ListCategoriesCommand(ITaskRepository repository) : ICommand
{
    /// <inheritdoc />
    public string Key => "7";

    /// <inheritdoc />
    public string Title => "Категории и теги";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var categories = await repository.GetCategoriesAsync(cancellationToken);

        if (categories.Count == 0)
        {
            Console.WriteLine("Категории ещё не созданы.");
            return;
        }

        Console.WriteLine("Категории:");
        foreach (var category in categories)
        {
            var color = string.IsNullOrWhiteSpace(category.Color) ? string.Empty : $" {category.Color}";
            Console.WriteLine($"  • {category.Name}{color}");
        }
    }
}
