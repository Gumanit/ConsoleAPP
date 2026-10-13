namespace ConsoleApp.Core.Domain;

/// <summary>
/// Состояние задачи в жизненном цикле.
/// </summary>
public enum TodoStatus
{
    /// <summary>Задача создана и ещё не начата.</summary>
    New,

    /// <summary>Задача выполняется.</summary>
    InProgress,

    /// <summary>Задача завершена.</summary>
    Done,

    /// <summary>Задача отменена.</summary>
    Cancelled,
}
