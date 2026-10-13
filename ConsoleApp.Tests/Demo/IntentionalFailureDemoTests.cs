namespace ConsoleApp.Tests.Demo;

/// <summary>
/// ВНИМАНИЕ: этот класс содержит НАМЕРЕННО падающий тест.
/// Он добавлен только для демонстрации того, как IDE отображает упавшие тесты
/// (красный результат, текст ошибки, переход к месту падения).
/// После проверки файл можно удалить.
/// </summary>
public sealed class IntentionalFailureDemoTests
{
    /// <summary>Проходит успешно — для контраста в дереве результатов.</summary>
    [Fact]
    public void Demo_Passing_Test()
    {
        Assert.True(true);
    }

    /// <summary>Намеренно падает: ожидание заведомо неверное.</summary>
    [Fact]
    public void Demo_Failing_Test_Finds_Too_Much()
    {
        var tasks = new[] { "Первая", "Вторая" };

        // Намеренная ошибка: проверяем, что задач ровно 5, хотя их 2.
        Assert.Equal(5, tasks.Length);
    }
}
