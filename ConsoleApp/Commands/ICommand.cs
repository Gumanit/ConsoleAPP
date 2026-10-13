namespace ConsoleApp.Commands;

/// <summary>
/// Пункт консольного меню, выполняющий одно законченное действие.
/// </summary>
public interface ICommand
{
    /// <summary>Номер пункта меню (ввод пользователя).</summary>
    string Key
    {
        get;
    }

    /// <summary>Заголовок пункта меню.</summary>
    string Title
    {
        get;
    }

    /// <summary>Выполняет действие команды.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
