using ConsoleApp.Core.Data;
using ConsoleApp.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Core.Repositories;

/// <summary>
/// Реализация <see cref="ITaskRepository"/> на основе Entity Framework Core.
/// </summary>
/// <param name="db">Контекст базы данных.</param>
public sealed class EfTaskRepository(TodoDbContext db) : ITaskRepository
{
    /// <inheritdoc />
    public async Task<TodoTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Tasks
            .Include(t => t.Category)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<TodoTask>> GetAllAsync(
        TaskFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<TodoTask> query = db.Tasks
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Tags);

        if (filter.Status is { } status)
        {
            query = query.Where(t => t.Status == status);
        }

        if (filter.Priority is { } priority)
        {
            query = query.Where(t => t.Priority == priority);
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(t => t.Category != null && t.Category.Name == filter.Category);
        }

        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            query = query.Where(t => t.Tags.Any(g => g.Name == filter.Tag));
        }

        if (filter.OnlyOverdue)
        {
            var now = DateTimeOffset.UtcNow;
            query = query.Where(t =>
                t.DueDate != null &&
                t.DueDate < now &&
                t.Status != TodoStatus.Done &&
                t.Status != TodoStatus.Cancelled);
        }

        return await query
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TodoTask>> SearchAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var pattern = $"%{text.Trim()}%";

        return await db.Tasks
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Tags)
            .Where(t =>
                EF.Functions.ILike(t.Title, pattern) ||
                (t.Description != null && EF.Functions.ILike(t.Description, pattern)))
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        await db.Tasks.AddAsync(task, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        db.Tasks.Update(task);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var affected = await db.Tasks
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return affected > 0;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(
        CancellationToken cancellationToken = default) =>
        await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Category> GetOrCreateCategoryAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var trimmed = name.Trim();
        var existing = await db.Categories.FirstOrDefaultAsync(c => c.Name == trimmed, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var created = new Category { Name = trimmed };
        db.Categories.Add(created);
        await db.SaveChangesAsync(cancellationToken);
        return created;
    }

    /// <inheritdoc />
    public async Task<Tag> GetOrCreateTagAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var trimmed = name.Trim();
        var existing = await db.Tags.FirstOrDefaultAsync(g => g.Name == trimmed, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var created = new Tag { Name = trimmed };
        db.Tags.Add(created);
        await db.SaveChangesAsync(cancellationToken);
        return created;
    }
}
