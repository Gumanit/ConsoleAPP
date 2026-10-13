using ConsoleApp.Core.Domain;
using ConsoleApp.Core.Repositories;

namespace ConsoleApp.Tests.Fakes;

/// <summary>
/// Простая реализация <see cref="ITaskRepository"/> в памяти — для модульных тестов без базы данных.
/// </summary>
internal sealed class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TodoTask> _tasks = [];
    private readonly List<Category> _categories = [];
    private readonly List<Tag> _tags = [];

    public IReadOnlyList<TodoTask> Tasks => _tasks;

    public Task<TodoTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_tasks.FirstOrDefault(task => task.Id == id));

    public Task<IReadOnlyList<TodoTask>> GetAllAsync(
        TaskFilter filter,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<TodoTask> query = _tasks;

        if (filter.Status is { } status)
        {
            query = query.Where(task => task.Status == status);
        }

        if (filter.Priority is { } priority)
        {
            query = query.Where(task => task.Priority == priority);
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(task => task.Category?.Name == filter.Category);
        }

        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            query = query.Where(task => task.Tags.Any(tag => tag.Name == filter.Tag));
        }

        if (filter.OnlyOverdue)
        {
            var now = DateTimeOffset.UtcNow;
            query = query.Where(task => task.IsOverdue(now));
        }

        return Task.FromResult<IReadOnlyList<TodoTask>>(query.ToList());
    }

    public Task<IReadOnlyList<TodoTask>> SearchAsync(
        string text,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<TodoTask>>(_tasks
            .Where(task =>
                task.Title.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                (task.Description?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList());

    public Task AddAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        _tasks.Add(task);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TodoTask task, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_tasks.RemoveAll(task => task.Id == id) > 0);

    public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Category>>(_categories.ToList());

    public Task<Category> GetOrCreateCategoryAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var existing = _categories.FirstOrDefault(category => category.Name == name);
        if (existing is not null)
        {
            return Task.FromResult(existing);
        }

        var created = new Category { Name = name };
        _categories.Add(created);
        return Task.FromResult(created);
    }

    public Task<Tag> GetOrCreateTagAsync(string name, CancellationToken cancellationToken = default)
    {
        var existing = _tags.FirstOrDefault(tag => tag.Name == name);
        if (existing is not null)
        {
            return Task.FromResult(existing);
        }

        var created = new Tag { Name = name };
        _tags.Add(created);
        return Task.FromResult(created);
    }
}
