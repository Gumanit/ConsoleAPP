using ConsoleApp.Core.Domain;
using ConsoleApp.Core.Extensions;

namespace ConsoleApp.Menu;

/// <summary>
/// Форматирование вывода списка задач в консоль.
/// </summary>
public static class ConsoleOutput
{
    /// <summary>Печатает список задач в виде таблицы.</summary>
    /// <param name="tasks">Задачи для вывода.</param>
    public static void PrintTasks(IReadOnlyList<TodoTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        if (tasks.Count == 0)
        {
            Console.WriteLine("Задачи не найдены.");
            return;
        }

        Console.WriteLine($"Найдено задач: {tasks.Count}.");
        Console.WriteLine(new string('-', 80));

        foreach (var task in tasks.Ordered())
        {
            Console.WriteLine($" {task.Id}  {task.Describe()}");
        }

        Console.WriteLine(new string('-', 80));
    }
}
