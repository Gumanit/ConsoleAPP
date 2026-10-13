using System.Diagnostics;

namespace ConsoleApp.Core.Domain;

/// <summary>
/// Основная сущность приложения — задача списка дел.
/// </summary>
[DebuggerDisplay("{Title} [{Status}] (Priority={Priority})")]
public sealed class TodoTask
{
    /// <summary>Уникальный идентификатор задачи.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Краткое название задачи.</summary>
    public required string Title
    {
        get; set;
    }

    /// <summary>Подробное описание (необязательно).</summary>
    public string? Description
    {
        get; set;
    }

    /// <summary>Приоритет задачи.</summary>
    public Priority Priority { get; set; } = Priority.Medium;

    /// <summary>Текущее состояние задачи.</summary>
    public TodoStatus Status { get; set; } = TodoStatus.New;

    /// <summary>Момент создания задачи.</summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Срок выполнения (необязательно).</summary>
    public DateTimeOffset? DueDate
    {
        get; set;
    }

    /// <summary>Момент завершения задачи.</summary>
    public DateTimeOffset? CompletedAt
    {
        get; set;
    }

    /// <summary>Идентификатор категории.</summary>
    public Guid? CategoryId
    {
        get; set;
    }

    /// <summary>Категория задачи.</summary>
    public Category? Category
    {
        get; set;
    }

    /// <summary>Набор тегов задачи.</summary>
    public List<Tag> Tags { get; } = [];

    /// <summary>
    /// Определяет, просрочена ли задача относительно указанного момента.
    /// </summary>
    /// <param name="now">Текущий момент времени.</param>
    /// <returns><see langword="true"/>, если задача просрочена.</returns>
    public bool IsOverdue(DateTimeOffset now) =>
        DueDate is { } due && Status is not (TodoStatus.Done or TodoStatus.Cancelled) && due < now;

    /// <summary>
    /// Помечает задачу выполненной.
    /// </summary>
    /// <param name="now">Момент завершения.</param>
    public void Complete(DateTimeOffset now)
    {
        Status = TodoStatus.Done;
        CompletedAt = now;
    }
}
