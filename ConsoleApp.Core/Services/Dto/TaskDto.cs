using ConsoleApp.Core.Domain;

namespace ConsoleApp.Core.Services.Dto;

/// <summary>
/// Транспортная модель задачи для импорта и экспорта JSON.
/// </summary>
public sealed record TaskDto
{
    /// <summary>Название задачи.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Описание задачи.</summary>
    public string? Description
    {
        get; init;
    }

    /// <summary>Приоритет в виде текста.</summary>
    public string Priority { get; init; } = nameof(Domain.Priority.Medium);

    /// <summary>Состояние в виде текста.</summary>
    public string Status { get; init; } = nameof(TodoStatus.New);

    /// <summary>Срок выполнения.</summary>
    public DateTimeOffset? DueDate
    {
        get; init;
    }

    /// <summary>Название категории.</summary>
    public string? Category
    {
        get; init;
    }

    /// <summary>Список тегов.</summary>
    public IReadOnlyList<string> Tags { get; init; } = [];
}
