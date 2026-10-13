using System.Text.Json;
using System.Text.RegularExpressions;
using ConsoleApp.Core.Domain;
using ConsoleApp.Core.Repositories;
using ConsoleApp.Core.Services.Dto;

namespace ConsoleApp.Core.Services;

/// <summary>
/// Прикладная логика работы с задачами: создание, поиск, завершение, статистика и обмен JSON.
/// </summary>
/// <param name="repository">Хранилище задач.</param>
public sealed partial class TodoService(ITaskRepository repository)
{
    /// <summary>Возвращает задачи согласно фильтру.</summary>
    /// <param name="filter">Критерии фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public Task<IReadOnlyList<TodoTask>> ListAsync(
        TaskFilter filter,
        CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(filter, cancellationToken);

    /// <summary>Выполняет поиск задач по тексту.</summary>
    /// <param name="text">Строка поиска.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public Task<IReadOnlyList<TodoTask>> SearchAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        return repository.SearchAsync(text.Trim(), cancellationToken);
    }

    /// <summary>Создаёт новую задачу.</summary>
    /// <param name="title">Название.</param>
    /// <param name="description">Описание.</param>
    /// <param name="priority">Приоритет.</param>
    /// <param name="dueDate">Срок выполнения.</param>
    /// <param name="category">Категория.</param>
    /// <param name="tags">Теги, разделённые запятой или точкой с запятой.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task<TodoTask> AddAsync(
        string title,
        string? description = null,
        Priority priority = Priority.Medium,
        DateTimeOffset? dueDate = null,
        string? category = null,
        string? tags = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Название задачи обязательно.", nameof(title));
        }

        var task = new TodoTask
        {
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Priority = priority,
            DueDate = dueDate,
            Status = TodoStatus.New,
        };

        if (!string.IsNullOrWhiteSpace(category))
        {
            task.Category = await repository.GetOrCreateCategoryAsync(category.Trim(), cancellationToken);
        }

        foreach (var tagName in ParseTags(tags))
        {
            task.Tags.Add(await repository.GetOrCreateTagAsync(tagName, cancellationToken));
        }

        await repository.AddAsync(task, cancellationToken);
        return task;
    }

    /// <summary>Помечает задачу выполненной.</summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns><see langword="true"/>, если задача найдена и завершена.</returns>
    public async Task<bool> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return false;
        }

        task.Complete(DateTimeOffset.UtcNow);
        await repository.UpdateAsync(task, cancellationToken);
        return true;
    }

    /// <summary>Удаляет задачу.</summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);

    /// <summary>Возвращает агрегированную статистику по всем задачам.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task<Statistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await repository.GetAllAsync(new TaskFilter(), cancellationToken);
        var now = DateTimeOffset.UtcNow;

        var byPriority = tasks
            .GroupBy(t => t.Priority.ToString())
            .ToDictionary(group => group.Key, group => group.Count());

        var byCategory = tasks
            .GroupBy(t => t.Category?.Name ?? "Без категории")
            .ToDictionary(group => group.Key, group => group.Count());

        return new Statistics(
            Total: tasks.Count,
            Done: tasks.Count(t => t.Status == TodoStatus.Done),
            InProgress: tasks.Count(t => t.Status == TodoStatus.InProgress),
            Overdue: tasks.Count(t => t.IsOverdue(now)),
            ByPriority: byPriority,
            ByCategory: byCategory);
    }

    /// <summary>Экспортирует все задачи в JSON-файл.</summary>
    /// <param name="path">Путь к файлу.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task ExportJsonAsync(string path, CancellationToken cancellationToken = default)
    {
        var tasks = await repository.GetAllAsync(new TaskFilter(), cancellationToken);
        var payload = tasks.Select(ToDto).ToList();

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, payload, TodoJsonContext.Default.ListTaskDto, cancellationToken);
    }

    /// <summary>Импортирует задачи из JSON-файла.</summary>
    /// <param name="path">Путь к файлу.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Количество импортированных задач.</returns>
    public async Task<int> ImportJsonAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(path);
        var payload = await JsonSerializer.DeserializeAsync(
            stream,
            TodoJsonContext.Default.ListTaskDto,
            cancellationToken) ?? [];

        return await ImportAsync(payload, cancellationToken);
    }

    /// <summary>Импортирует набор задач.</summary>
    /// <param name="items">Задачи для импорта.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Количество импортированных задач.</returns>
    public async Task<int> ImportAsync(
        IEnumerable<TaskDto> items,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);

        var count = 0;
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                continue;
            }

            var priority = Enum.TryParse<Priority>(item.Priority, ignoreCase: true, out var parsed)
                ? parsed
                : Priority.Medium;

            await AddAsync(
                item.Title,
                item.Description,
                priority,
                item.DueDate,
                item.Category,
                string.Join(',', item.Tags),
                cancellationToken);

            count++;
        }

        return count;
    }

    private static IEnumerable<string> ParseTags(string? tags) =>
        string.IsNullOrWhiteSpace(tags)
            ? []
            : TagSeparator()
                .Split(tags)
                .Select(tag => tag.Trim())
                .Where(tag => tag.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase);

    private static TaskDto ToDto(TodoTask task) => new()
    {
        Title = task.Title,
        Description = task.Description,
        Priority = task.Priority.ToString(),
        Status = task.Status.ToString(),
        DueDate = task.DueDate,
        Category = task.Category?.Name,
        Tags = task.Tags.Select(tag => tag.Name).ToList(),
    };

    [GeneratedRegex(@"[,;]")]
    private static partial Regex TagSeparator();
}
