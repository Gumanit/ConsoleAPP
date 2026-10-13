using ConsoleApp.Core.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ConsoleApp.Tests.Repositories;

/// <summary>
/// Фикстура, поднимающая одноразовый контейнер PostgreSQL для интеграционных тестов.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("todo_test")
        .WithUsername("todo")
        .WithPassword("todo")
        .Build();

    /// <inheritdoc />
    public Task InitializeAsync() => _container.StartAsync();

    /// <inheritdoc />
    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    /// <summary>Создаёт новый контекст на чистой базе данных.</summary>
    public async Task<TodoDbContext> CreateFreshContextAsync()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        var context = new TodoDbContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
        return context;
    }
}
