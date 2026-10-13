using ConsoleApp.Core.Domain;
using ConsoleApp.Core.Services;

namespace ConsoleApp.Tests.Services;

/// <summary>Модульные тесты утилит из <see cref="LanguageFeatures"/>.</summary>
public sealed class LanguageFeaturesTests
{
    [Theory]
    [InlineData(Priority.Critical, "критический")]
    [InlineData(Priority.High, "высокий")]
    [InlineData(Priority.Medium, "средний")]
    [InlineData(Priority.Low, "низкий")]
    public void DescribePriority_ReturnsExpectedText(Priority priority, string expected) =>
        Assert.Equal(expected, LanguageFeatures.DescribePriority(priority));

    [Theory]
    [InlineData(1, "1 задача")]
    [InlineData(2, "2 задачи")]
    [InlineData(5, "5 задач")]
    [InlineData(11, "11 задач")]
    [InlineData(21, "21 задача")]
    [InlineData(112, "112 задач")]
    public void PluralizeTasks_ReturnsCorrectForm(int count, string expected) =>
        Assert.Equal(expected, LanguageFeatures.PluralizeTasks(count));

    [Fact]
    public void DescribePriority_Throws_OnUnknownValue() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => LanguageFeatures.DescribePriority((Priority)99));
}
