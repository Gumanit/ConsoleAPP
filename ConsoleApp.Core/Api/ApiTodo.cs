using System.Text.Json.Serialization;

namespace ConsoleApp.Core.Api;

/// <summary>
/// Внешняя модель задачи, получаемая из публичного REST API jsonplaceholder.
/// </summary>
public sealed record ApiTodo
{
    /// <summary>Идентификатор записи.</summary>
    [JsonPropertyName("id")]
    public int Id
    {
        get; init;
    }

    /// <summary>Идентификатор пользователя.</summary>
    [JsonPropertyName("userId")]
    public int UserId
    {
        get; init;
    }

    /// <summary>Заголовок.</summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    /// <summary>Признак завершения.</summary>
    [JsonPropertyName("completed")]
    public bool Completed
    {
        get; init;
    }
}
