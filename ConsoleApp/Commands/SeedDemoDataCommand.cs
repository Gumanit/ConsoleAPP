using ConsoleApp.Core.Domain;
using ConsoleApp.Core.Services;

namespace ConsoleApp.Commands;

/// <summary>Наполняет базу демонстрационными задачами (удобно для проверки отладки и статистики).</summary>
internal sealed class SeedDemoDataCommand(TodoService service) : ICommand
{
    private static readonly string[] Titles =
    [
        "Подготовить план спринта",
        "Согласовать требования с заказчиком",
        "Исправить найденные баги",
        "Обновить документацию",
        "Провести код-ревью",
        "Настроить CI-конвейер",
        "Разобрать почту и задачи",
        "Провести планёрку команды",
    ];

    /// <inheritdoc />
    public string Key => "11";

    /// <inheritdoc />
    public string Title => "Загрузить демонстрационные данные";

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var created = 0;

        for (var index = 0; index < Titles.Length; index++)
        {
            var priority = (Priority)((index % 4) + 1);
            var dueDate = now.AddDays(index - 3);
            var category = index % 2 == 0 ? "Работа" : "Личное";
            var tags = index % 3 == 0 ? "важное,срочно" : "демо";

            await service.AddAsync(
                Titles[index],
                $"Демонстрационная задача №{index + 1}",
                priority,
                dueDate,
                category,
                tags,
                cancellationToken);

            created++;
        }

        Console.WriteLine($"Добавлено демонстрационных задач: {created}.");
    }
}
