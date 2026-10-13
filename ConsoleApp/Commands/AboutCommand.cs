using System.Runtime.InteropServices;
using ConsoleApp.Core.Services;

namespace ConsoleApp.Commands;

/// <summary>Показывает сведения о приложении и среде выполнения.</summary>
internal sealed class AboutCommand : ICommand
{
    /// <inheritdoc />
    public string Key => "12";

    /// <inheritdoc />
    public string Title => "О приложении и среде выполнения";

    /// <inheritdoc />
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine(LanguageFeatures.HelpText);
        Console.WriteLine();
        Console.WriteLine($"Версия .NET:      {Environment.Version}");
        Console.WriteLine($"Framework:        {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"Операционная с-ма: {RuntimeInformation.OSDescription}");
        Console.WriteLine($"RID:              {RuntimeInformation.RuntimeIdentifier}");
        Console.WriteLine($"Архитектура:      {RuntimeInformation.ProcessArchitecture}");
        Console.WriteLine($"Процессоров:      {Environment.ProcessorCount}");
        Console.WriteLine($"Каталог сборки:   {AppContext.BaseDirectory}");

        return Task.CompletedTask;
    }
}
