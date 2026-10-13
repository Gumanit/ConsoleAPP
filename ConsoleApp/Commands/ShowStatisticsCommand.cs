using ConsoleApp.Core.Services;

namespace ConsoleApp.Commands;

/// <summary>Выводит агрегированную статистику по задачам.</summary>
internal sealed class ShowStatisticsCommand(TodoService service) : ICommand
{
    /// <inheritdoc />
    public string Key => "6";

    /// <inheritdoc />
    public string Title => "Статистика по задачам";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var statistics = await service.GetStatisticsAsync(cancellationToken);

        Console.WriteLine($"Всего задач:      {statistics.Total}");
        Console.WriteLine($"Выполнено:        {statistics.Done}");
        Console.WriteLine($"В работе:         {statistics.InProgress}");
        Console.WriteLine($"Просрочено:       {statistics.Overdue}");
        Console.WriteLine($"Доля выполнения:  {statistics.CompletionRate:P0}");

        Console.WriteLine();
        Console.WriteLine("По приоритетам:");
        foreach (var (priority, count) in statistics.ByPriority)
        {
            Console.WriteLine($"  {priority,-9} {count}");
        }

        Console.WriteLine();
        Console.WriteLine("По категориям:");
        foreach (var (category, count) in statistics.ByCategory)
        {
            Console.WriteLine($"  {category,-20} {count}");
        }
    }
}
