using ConsoleApp.Core.Domain;

namespace ConsoleApp.Core.Repositories;

/// <summary>
/// Критерии фильтрации списка задач.
/// </summary>
public sealed record TaskFilter
{
    /// <summary>Фильтр по состоянию.</summary>
    public TodoStatus? Status
    {
        get; init;
    }

    /// <summary>Фильтр по приоритету.</summary>
    public Priority? Priority
    {
        get; init;
    }

    /// <summary>Фильтр по названию категории.</summary>
    public string? Category
    {
        get; init;
    }

    /// <summary>Фильтр по тегу.</summary>
    public string? Tag
    {
        get; init;
    }

    /// <summary>Показывать только просроченные задачи.</summary>
    public bool OnlyOverdue
    {
        get; init;
    }
}
