using System.Text.Json.Serialization;

namespace ConsoleApp.Core.Api;

/// <summary>
/// Контекст source generation для десериализации ответа внешнего API.
/// </summary>
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, WriteIndented = true)]
[JsonSerializable(typeof(ApiTodo))]
[JsonSerializable(typeof(List<ApiTodo>))]
public sealed partial class ApiTodoJsonContext : JsonSerializerContext;
