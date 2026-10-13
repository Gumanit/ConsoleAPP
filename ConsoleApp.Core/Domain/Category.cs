namespace ConsoleApp.Core.Domain;

/// <summary>
/// Категория (проект), объединяющая задачи.
/// </summary>
public sealed class Category
{
    /// <summary>Уникальный идентификатор категории.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Название категории.</summary>
    public required string Name
    {
        get; set;
    }

    /// <summary>Цвет для подсветки в интерфейсе (hex-код).</summary>
    public string? Color
    {
        get; set;
    }

    /// <summary>Задачи, входящие в категорию.</summary>
    public List<TodoTask> Tasks { get; } = [];
}
