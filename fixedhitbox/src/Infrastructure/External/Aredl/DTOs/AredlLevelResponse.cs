using System.Text.Json.Serialization;

namespace fixedhitbox.Infrastructure.External.Aredl.DTOs;

public sealed record AredllevelResponse
{
    public Guid Id { get; }
    public string? Name { get; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public ulong? LevelId { get; }

    public bool? TwoPlayer { get; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Position { get; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Points { get; }

    public bool? Legacy { get; }
}