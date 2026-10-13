using System.Text.Json.Serialization;

namespace ConsoleApp.Core.Services.Dto;

/// <summary>
/// Контекст source generation для сериализации <see cref="TaskDto"/>.
/// Генератор создаёт код сериализации во время сборки — без рефлексии в рантайме.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(TaskDto))]
[JsonSerializable(typeof(List<TaskDto>))]
public sealed partial class TodoJsonContext : JsonSerializerContext;
