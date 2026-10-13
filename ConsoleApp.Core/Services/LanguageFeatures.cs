using ConsoleApp.Core.Domain;

namespace ConsoleApp.Core.Services;

/// <summary>
/// Небольшая витрина возможностей современного C# (критерий «версионность языка»).
/// </summary>
public static class LanguageFeatures
{
    /// <summary>
    /// Возвращает человекочитаемое название приоритета (switch expression + сопоставление с образцом).
    /// </summary>
    /// <param name="priority">Приоритет.</param>
    public static string DescribePriority(Priority priority) => priority switch
    {
        Priority.Critical => "критический",
        Priority.High => "высокий",
        Priority.Medium => "средний",
        Priority.Low => "низкий",
        _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, "Неизвестный приоритет."),
    };

    /// <summary>
    /// Возвращает строку с корректным склонением слова «задача».
    /// </summary>
    /// <param name="count">Количество задач.</param>
    public static string PluralizeTasks(int count) => (count % 10) switch
    {
        1 when count % 100 != 11 => $"{count} задача",
        2 or 3 or 4 when count % 100 is < 12 or > 14 => $"{count} задачи",
        _ => $"{count} задач",
    };

    /// <summary>Краткая справка о приложении (raw string literal).</summary>
    public static string HelpText => """
        Менеджер задач (TODO) — демонстрационное консольное приложение.
        Поддерживает категории, теги, приоритеты, сроки и отчёты.
        Данные хранятся в PostgreSQL и доступны через Entity Framework Core.
        """;
}
