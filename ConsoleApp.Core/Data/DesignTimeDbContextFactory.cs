using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConsoleApp.Core.Data;

/// <summary>
/// Фабрика контекста, используемая инструментами проектирования (например, <c>dotnet ef</c>).
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TodoDbContext>
{
    /// <summary>Строка подключения по умолчанию (совпадает с docker-compose.yml).</summary>
    public const string DefaultConnectionString =
        "Host=localhost;Port=5433;Database=todo_db;Username=todo;Password=todo";

    /// <inheritdoc />
    public TodoDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("TODO_DB_CONNECTION") ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TodoDbContext(options);
    }
}
