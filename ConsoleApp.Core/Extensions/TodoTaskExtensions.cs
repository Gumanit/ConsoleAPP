using ConsoleApp.Core.Domain;

namespace ConsoleApp.Core.Extensions;

/// <summary>
/// Методы расширения для <see cref="TodoTask"/> и последовательностей задач.
/// Демонстрируют поддержку IntelliSense для extension methods.
/// </summary>
public static class TodoTaskExtensions
{
    /// <summary>
    /// Формирует однострочное текстовое описание задачи для вывода в консоль.
    /// </summary>
    /// <param name="task">Задача.</param>
    public static string Describe(this TodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var category = task.Category?.Name ?? "Без категории";
        var due = task.DueDate is { } value
            ? value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
            : "без срока";
        var tags = task.Tags.Count > 0
            ? $" #{string.Join(" #", task.Tags.Select(tag => tag.Name))}"
            : string.Empty;

        return $"[{task.Status}] {task.Title} · {task.Priority} · {category} · до {due}{tags}";
    }

    /// <summary>Оставляет только активные задачи (не выполненные и не отменённые).</summary>
    /// <param name="tasks">Исходная последовательность.</param>
    public static IEnumerable<TodoTask> OnlyActive(this IEnumerable<TodoTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        return tasks.Where(task => task.Status is not (TodoStatus.Done or TodoStatus.Cancelled));
    }

    /// <summary>Сортирует задачи по приоритету (убывание) и сроку (возрастание).</summary>
    /// <param name="tasks">Исходная последовательность.</param>
    public static IEnumerable<TodoTask> Ordered(this IEnumerable<TodoTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        return tasks
            .OrderByDescending(task => task.Priority)
            .ThenBy(task => task.DueDate);
    }
}
