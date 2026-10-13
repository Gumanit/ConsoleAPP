namespace ConsoleApp.Core.Services;

/// <summary>
/// Агрегированная статистика по задачам.
/// </summary>
/// <param name="Total">Всего задач.</param>
/// <param name="Done">Выполненных задач.</param>
/// <param name="InProgress">Задач в работе.</param>
/// <param name="Overdue">Просроченных задач.</param>
/// <param name="ByPriority">Распределение по приоритетам.</param>
/// <param name="ByCategory">Распределение по категориям.</param>
public sealed record Statistics(
    int Total,
    int Done,
    int InProgress,
    int Overdue,
    IReadOnlyDictionary<string, int> ByPriority,
    IReadOnlyDictionary<string, int> ByCategory)
{
    /// <summary>Доля выполненных задач в диапазоне от 0 до 1.</summary>
    public double CompletionRate => Total == 0 ? 0d : (double)Done / Total;
}
