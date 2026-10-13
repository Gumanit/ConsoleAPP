namespace ConsoleApp.Core.Domain;

/// <summary>
/// Тег для сквозной пометки задач (связь многие-ко-многим).
/// </summary>
public sealed class Tag
{
    /// <summary>Уникальный идентификатор тега.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Название тега.</summary>
    public required string Name
    {
        get; set;
    }

    /// <summary>Задачи, помеченные тегом.</summary>
    public List<TodoTask> Tasks { get; } = [];
}
