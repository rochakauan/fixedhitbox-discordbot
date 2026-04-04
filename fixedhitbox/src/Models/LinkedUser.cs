using System.Text.Json.Serialization;
using fixedhitbox.Dtos.Aredl;

namespace fixedhitbox.Models;

public sealed class LinkedUser
{
    [JsonIgnore]
    public int Id { get; init; }
    public ulong DiscordId { get; init; }

    public string Username { get; init; } = string.Empty;
    public string GlobalName { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;

    public Guid AredlUserId { get; init; }
    public int? Country { get; init; }
    public DateTime CreatedInAredlAt { get; init; }
    
    public DateTime LinkedAtUtc { get; private set; }
    public DateTime LastUpdateAtUtc { get; private set; }

    private LinkedUser() { }

    public static LinkedUser CreateFromPending(PendingAredlLinkDto dto)
        => new()
        {
            DiscordId = dto.DiscordId,
            Username = dto.Username,
            GlobalName = dto.GlobalName,
            Description = dto.Description,
            AredlUserId = dto.AredlUserId,
            Country = dto.Country,
            CreatedInAredlAt = dto.CreatedInAredlAt
        };
}