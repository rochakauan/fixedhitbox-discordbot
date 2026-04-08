using System.Text.Json.Serialization;

namespace fixedhitbox.Infrastructure.External.Aredl.DTOs;

public sealed record AredllevelResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public ulong? LevelId { get; init; }

    public bool? TwoPlayer { get; init; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Position { get; init; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Points { get; init; }

    public bool? Legacy { get; init; }
}