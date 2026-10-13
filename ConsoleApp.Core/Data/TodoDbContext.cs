using ConsoleApp.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Core.Data;

/// <summary>
/// Контекст Entity Framework Core для хранения задач, категорий и тегов в PostgreSQL.
/// </summary>
/// <param name="options">Параметры контекста.</param>
public sealed class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    /// <summary>Таблица задач.</summary>
    public DbSet<TodoTask> Tasks => Set<TodoTask>();

    /// <summary>Таблица категорий.</summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>Таблица тегов.</summary>
    public DbSet<Tag> Tags => Set<Tag>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
