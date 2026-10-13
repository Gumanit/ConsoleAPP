using System.Diagnostics;
using System.Windows.Input;
using ConsoleApp.Core.Domain;

namespace ConsoleApp.Menu;

/// <summary>
/// Вспомогательные методы чтения значений из консоли с разумными значениями по умолчанию.
/// </summary>
public static class ConsolePrompt
{
    /// <summary>Запрашивает строку; при пустом вводе возвращает значение по умолчанию.</summary>
    /// <param name="label">Подпись поля.</param>
    /// <param name="defaultValue">Значение по умолчанию.</param>
    public static string Ask(string label, string? defaultValue = null)
    {
        Console.Write(defaultValue is null ? $"{label}: " : $"{label} [{defaultValue}]: ");
        var value = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(value) ? defaultValue ?? string.Empty : value;
    }

    /// <summary>Запрашивает необязательную строку.</summary>
    /// <param name="label">Подпись поля.</param>
    public static string? AskOptional(string label)
    {
        var value = Ask(label);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    /// <summary>Запрашивает подтверждение да/нет.</summary>
    /// <param name="label">Подпись вопроса.</param>
    public static bool AskBool(string label)
    {
        Console.Write($"{label} (y/N): ");
        var value = Console.ReadLine()?.Trim();
        return value is not null
            && (value.Equals("y", StringComparison.OrdinalIgnoreCase)
                || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || value.Equals("д", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Запрашивает приоритет.</summary>
    /// <param name="defaultValue">Приоритет по умолчанию.</param>
    public static Priority AskPriority(Priority defaultValue = Priority.Medium)
    {
        var value = Ask("Приоритет (Low/Medium/High/Critical)", defaultValue.ToString());
        return Enum.TryParse<Priority>(value, ignoreCase: true, out var parsed) ? parsed : defaultValue;
    }

    /// <summary>Запрашивает состояние задачи или <see langword="null"/>, чтобы не фильтровать.</summary>
    public static TodoStatus? AskStatus()
    {
        var value = Ask("Состояние (New/InProgress/Done/Cancelled, пусто — все)", "все");
        return Enum.TryParse<TodoStatus>(value, ignoreCase: true, out var parsed) ? parsed : null;
    }

    /// <summary>Запрашивает необязательную категорию.</summary>
    public static string? AskCategory() => AskOptional("Категория (пусто — любая)");

    /// <summary>Запрашивает необязательный тег.</summary>
    public static string? AskTag() => AskOptional("Тег (пусто — любой)");

    /// <summary>Запрашивает целое число.</summary>
    /// <param name="label">Подпись поля.</param>
    /// <param name="defaultValue">Значение по умолчанию.</param>
    public static int AskInt(string label, int defaultValue)
    {
        var value = Ask(label, defaultValue.ToString());
        return int.TryParse(value, out var parsed) ? parsed : defaultValue;
    }

    /// <summary>Запрашивает дату или <see langword="null"/>.</summary>
    /// <param name="label">Подпись поля.</param>
    public static DateTimeOffset? AskDate(string label)
    {
        var value = AskOptional(label);
        return value is not null && DateTimeOffset.TryParse(value, out var parsed) ? parsed : null;
    }

    /// <summary>Запрашивает идентификатор задачи, повторяя ввод до корректного значения.</summary>
    /// <param name="label">Подпись поля.</param>
    public static Guid AskGuid(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var value = Console.ReadLine()?.Trim();
            if (Guid.TryParse(value, out var id))
            {
                return id;
            }

            Console.WriteLine("Некорректный идентификатор. Возьмите значение из списка задач.");
        }
    }
}


