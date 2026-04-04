using fixedhitbox.Models;

namespace fixedhitbox.Dtos.Aredl;

public sealed record PendingAredlLinkDto
{
    public ulong DiscordId { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string GlobalName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid AredlUserId { get; private set; }
    public int? Country {get; private set;}
    public DateTime CreatedInAredlAt { get; private set; }

    public static PendingAredlLinkDto Create(
        ulong discordId,
        string username,
        string globalName,
        string? description,
        Guid aredlUserId,
        int? country,
        DateTime createdInAredlAt)
        => new()
        {
            DiscordId = discordId,
            Username = username,
            GlobalName = globalName,
            Description = description,
            AredlUserId = aredlUserId,
            Country = country,
            CreatedInAredlAt = createdInAredlAt
        };

    public static PendingAredlLinkDto CreateFromEntity(LinkedUser entity)
        => new()
        {
            DiscordId = entity.DiscordId,
            Username = entity.Username,
            GlobalName = entity.GlobalName,
            Description = entity.Description,
            AredlUserId = entity.AredlUserId,
            Country = entity.Country,
            CreatedInAredlAt = entity.CreatedInAredlAt
        };
}