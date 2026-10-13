namespace ConsoleApp.Core.Domain;

/// <summary>
/// Приоритет задачи. Значение численно растёт вместе с важностью.
/// </summary>
public enum Priority
{
    /// <summary>Низкий приоритет.</summary>
    Low = 1,

    /// <summary>Обычный приоритет по умолчанию.</summary>
    Medium = 2,

    /// <summary>Высокий приоритет.</summary>
    High = 3,

    /// <summary>Критический приоритет, требует немедленного внимания.</summary>
    Critical = 4,
}
