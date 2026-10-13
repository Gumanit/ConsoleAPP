using ConsoleApp.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Menu;

/// <summary>
/// Интерактивное текстовое меню приложения.
/// </summary>
/// <param name="scopeFactory">Фабрика DI-областей для разрешения команд.</param>
public sealed class ConsoleMenu(IServiceScopeFactory scopeFactory)
{
    /// <summary>Запускает цикл обработки пользовательского ввода.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Менеджер задач (TODO) ===");
        Console.WriteLine("Введите номер пункта или 0 для выхода.");

        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var commands = scope.ServiceProvider
                .GetServices<ICommand>()
                .OrderBy(command => int.TryParse(command.Key, out var number) ? number : int.MaxValue)
                .ToList();

            RenderMenu(commands);

            Console.Write("Выберите пункт: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input) || input is "0" or "q" or "Q")
            {
                Console.WriteLine("До встречи!");
                return;
            }

            var selected = commands.FirstOrDefault(command => command.Key == input);
            if (selected is null)
            {
                Console.WriteLine("Неизвестный пункт меню. Повторите ввод.");
                continue;
            }

            Console.WriteLine();
            try
            {
                await selected.ExecuteAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[Ошибка] {exception.Message}");
            }

            Console.WriteLine();
            Console.Write("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }

    private static void RenderMenu(IReadOnlyList<ICommand> commands)
    {
        Console.WriteLine();

        foreach (var command in commands)
        {
            Console.WriteLine($" {command.Key,2}. {command.Title}");
        }
    }
}
