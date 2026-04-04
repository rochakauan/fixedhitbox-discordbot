using System.Text.Json.Serialization;

namespace fixedhitbox.Dtos.Aredl;

public sealed record AredlProfileResponse
{
    
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? GlobalName { get; init; }
    
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public ulong? DiscordId { get; init; }
    
    public string? Description { get; init; }
    
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Country { get; init; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedInAredlAt { get; init; }

}