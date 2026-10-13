using ConsoleApp.Core.Domain;

namespace ConsoleApp.Core.Repositories;

/// <summary>
/// Абстракция доступа к задачам, категориям и тегам.
/// </summary>
public interface ITaskRepository
{
    /// <summary>Возвращает задачу по идентификатору или <see langword="null"/>.</summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<TodoTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает задачи, удовлетворяющие фильтру.</summary>
    /// <param name="filter">Критерии фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<TodoTask>> GetAllAsync(TaskFilter filter, CancellationToken cancellationToken = default);

    /// <summary>Выполняет полнотекстовый поиск по названию и описанию.</summary>
    /// <param name="text">Строка поиска.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<TodoTask>> SearchAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>Добавляет задачу.</summary>
    /// <param name="task">Добавляемая задача.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task AddAsync(TodoTask task, CancellationToken cancellationToken = default);

    /// <summary>Обновляет задачу.</summary>
    /// <param name="task">Изменяемая задача.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task UpdateAsync(TodoTask task, CancellationToken cancellationToken = default);

    /// <summary>Удаляет задачу по идентификатору.</summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns><see langword="true"/>, если запись была удалена.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает список категорий.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>Возвращает категорию по имени, создавая её при необходимости.</summary>
    /// <param name="name">Название категории.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<Category> GetOrCreateCategoryAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Возвращает тег по имени, создавая его при необходимости.</summary>
    /// <param name="name">Название тега.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<Tag> GetOrCreateTagAsync(string name, CancellationToken cancellationToken = default);
}
