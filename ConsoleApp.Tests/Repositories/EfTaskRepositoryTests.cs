using ConsoleApp.Core.Domain;
using ConsoleApp.Core.Repositories;

namespace ConsoleApp.Tests.Repositories;

/// <summary>
/// Интеграционные тесты репозитория на реальном PostgreSQL (Testcontainers).
/// </summary>
public sealed class EfTaskRepositoryTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task AddAsync_PersistsTask_WithCategoryAndTags()
    {
        await using var db = await fixture.CreateFreshContextAsync();
        var repository = new EfTaskRepository(db);

        var category = await repository.GetOrCreateCategoryAsync("Работа");
        var tag = await repository.GetOrCreateTagAsync("срочно");

        var task = new TodoTask
        {
            Title = "Написать тесты",
            Description = "Проверить репозиторий",
            Priority = Priority.High,
            DueDate = DateTimeOffset.UtcNow.AddDays(1),
            Category = category,
        };
        task.Tags.Add(tag);

        await repository.AddAsync(task);

        var loaded = await repository.GetByIdAsync(task.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Написать тесты", loaded!.Title);
        Assert.Equal("Работа", loaded.Category?.Name);
        Assert.Contains(loaded.Tags, item => item.Name == "срочно");
    }

    [Fact]
    public async Task GetAllAsync_FiltersByPriority()
    {
        await using var db = await fixture.CreateFreshContextAsync();
        var repository = new EfTaskRepository(db);

        await repository.AddAsync(new TodoTask { Title = "Высокая", Priority = Priority.High });
        await repository.AddAsync(new TodoTask { Title = "Низкая", Priority = Priority.Low });

        var tasks = await repository.GetAllAsync(new TaskFilter { Priority = Priority.High });

        Assert.Single(tasks);
        Assert.Equal("Высокая", tasks[0].Title);
    }

    [Fact]
    public async Task SearchAsync_MatchesTitleCaseInsensitively()
    {
        await using var db = await fixture.CreateFreshContextAsync();
        var repository = new EfTaskRepository(db);

        await repository.AddAsync(new TodoTask { Title = "Обновить Документацию" });
        await repository.AddAsync(new TodoTask { Title = "Написать код" });

        var tasks = await repository.SearchAsync("документ");

        Assert.Single(tasks);
        Assert.Equal("Обновить Документацию", tasks[0].Title);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingTask()
    {
        await using var db = await fixture.CreateFreshContextAsync();
        var repository = new EfTaskRepository(db);

        var task = new TodoTask { Title = "Удаляемая" };
        await repository.AddAsync(task);

        var deleted = await repository.DeleteAsync(task.Id);

        Assert.True(deleted);
        Assert.Null(await repository.GetByIdAsync(task.Id));
    }
}
