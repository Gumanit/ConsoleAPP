namespace ConsoleApp.Core.Api;

/// <summary>
/// Заготовленные данные на случай, когда внешний API недоступен (offline-фолбэк).
/// </summary>
public static class OfflineTodoFallback
{
    /// <summary>Возвращает статический набор демонстрационных задач.</summary>
    public static IReadOnlyList<ApiTodo> CreateDefault() =>
    [
        new() { Id = 1, UserId = 1, Title = "delectus aut autem", Completed = false },
        new() { Id = 2, UserId = 1, Title = "quis ut nam facilis et officia qui", Completed = false },
        new() { Id = 3, UserId = 1, Title = "fugiat veniam minus", Completed = false },
        new() { Id = 4, UserId = 1, Title = "et porro tempora", Completed = true },
        new() { Id = 5, UserId = 1, Title = "laboriosam mollitia et enim quasi adipisci quia provident", Completed = false },
    ];
}
