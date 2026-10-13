using ConsoleApp.Core.Domain;

namespace ConsoleApp.Tests.Domain;

/// <summary>Модульные тесты поведения сущности <see cref="TodoTask"/>.</summary>
public sealed class TodoTaskTests
{
    [Fact]
    public void IsOverdue_ReturnsTrue_WhenDueDateInPast_AndTaskIsActive()
    {
        var now = DateTimeOffset.UtcNow;
        var task = new TodoTask { Title = "Просроченная", DueDate = now.AddHours(-1) };

        Assert.True(task.IsOverdue(now));
    }

    [Fact]
    public void IsOverdue_ReturnsFalse_WhenTaskIsCompleted()
    {
        var now = DateTimeOffset.UtcNow;
        var task = new TodoTask { Title = "Готовая", DueDate = now.AddHours(-1) };

        task.Complete(now);

        Assert.False(task.IsOverdue(now));
    }

    [Fact]
    public void IsOverdue_ReturnsFalse_WhenDueDateIsInFuture()
    {
        var now = DateTimeOffset.UtcNow;
        var task = new TodoTask { Title = "Будущая", DueDate = now.AddDays(1) };

        Assert.False(task.IsOverdue(now));
    }

    [Fact]
    public void Complete_SetsStatusAndCompletionTime()
    {
        var now = DateTimeOffset.UtcNow;
        var task = new TodoTask { Title = "Задача" };

        task.Complete(now);

        Assert.Equal(TodoStatus.Done, task.Status);
        Assert.Equal(now, task.CompletedAt);
    }
}
