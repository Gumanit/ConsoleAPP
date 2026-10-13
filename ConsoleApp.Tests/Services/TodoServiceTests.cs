using ConsoleApp.Core.Domain;
using ConsoleApp.Core.Services;
using ConsoleApp.Core.Services.Dto;
using ConsoleApp.Tests.Fakes;

namespace ConsoleApp.Tests.Services;

/// <summary>Модульные тесты прикладной логики <see cref="TodoService"/>.</summary>
public sealed class TodoServiceTests
{
    [Fact]
    public async Task AddAsync_Throws_WhenTitleIsMissing()
    {
        var service = new TodoService(new InMemoryTaskRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync("   "));
    }

    [Fact]
    public async Task AddAsync_ParsesTags_AndAssignsCategory()
    {
        var repository = new InMemoryTaskRepository();
        var service = new TodoService(repository);

        var task = await service.AddAsync("Тестовая задача", tags: "срочно, важно", category: "Работа");

        Assert.Equal("Работа", task.Category?.Name);
        Assert.Equal(2, task.Tags.Count);
        Assert.Single(repository.Tasks);
    }

    [Fact]
    public async Task GetStatisticsAsync_ComputesCountsAndRate()
    {
        var repository = new InMemoryTaskRepository();
        var service = new TodoService(repository);

        var first = await service.AddAsync("Первая", priority: Priority.High);
        await service.AddAsync("Вторая");
        await service.CompleteAsync(first.Id);

        var statistics = await service.GetStatisticsAsync();

        Assert.Equal(2, statistics.Total);
        Assert.Equal(1, statistics.Done);
        Assert.Equal(0.5d, statistics.CompletionRate);
    }

    [Fact]
    public async Task ImportAsync_SkipsEmptyTitles_AndParsesPriority()
    {
        var repository = new InMemoryTaskRepository();
        var service = new TodoService(repository);

        var items = new List<TaskDto>
        {
            new() { Title = "Из API", Priority = "High" },
            new() { Title = "   " },
        };

        var imported = await service.ImportAsync(items);

        Assert.Equal(1, imported);
        Assert.Equal(Priority.High, repository.Tasks.Single().Priority);
    }
}
