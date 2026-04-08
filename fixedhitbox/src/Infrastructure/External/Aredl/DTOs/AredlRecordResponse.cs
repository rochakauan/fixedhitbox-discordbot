using System.Text.Json.Serialization;
namespace fixedhitbox.Infrastructure.External.Aredl.DTOs;

public sealed record AredlRecordResponse
{
    public Guid Id { get; init; }
    
    [JsonPropertyName("level")]
    public AredllevelResponse? Level { get; init; }

    [JsonPropertyName("mobile")]
    public bool? Mobile { get; init; }
    public string? VideoUrl { get; init; }
    public bool? IsVerification { get; init; }
    public bool? HideVideo { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}