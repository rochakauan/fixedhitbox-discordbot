using fixedhitbox.Domain.Entities;
using fixedhitbox.Shared;
using fixedhitbox.Shared.Results.Mappers;

namespace fixedhitbox.Domain.Domain_DTOs;

[Obsolete("Subject to removal in the next stable release. Review this object's dependencies.")]
public sealed class PendingAredlLink
{
    public ulong DiscordId { get; }
    public string Username { get; }
    public string GlobalName { get; }
    public string? Description { get; }
    public Guid AredlUserId { get; }
    public int? Country { get; }
    public DateTime CreatedInAredlAt { get; }
    
    private PendingAredlLink(
        ulong discordId,
        string username,
        string globalName,
        string? description,
        Guid aredlUserId,
        int? country,
        DateTime createdInAredlAt)
    {
        DiscordId = discordId;
        Username = username;
        GlobalName = globalName;
        Description = description;
        AredlUserId = aredlUserId;
        Country = country;
        CreatedInAredlAt = createdInAredlAt;
    }

    public static MapperResult<PendingAredlLink> Create(
        ulong? discordId,
        string? username,
        string? globalName,
        string? description,
        Guid id,
        int? country,
        DateTime? createdAt)
    {
        if (discordId is null)
            return MapperResult<PendingAredlLink>.Fail("DiscordId is missing.");
        if (string.IsNullOrWhiteSpace(username))
            return MapperResult<PendingAredlLink>.Fail("Username is empty or whitespace.");
        
        if (createdAt is null)
            return MapperResult<PendingAredlLink>.Fail("CreatedAt is missing.");

        return MapperResult<PendingAredlLink>.Ok(new PendingAredlLink(
            discordId.Value,
            username,
            globalName ?? string.Empty,
            description,
            id,
            country,
            createdAt.Value));
    }

    public static PendingAredlLink CreateFromEntity(LinkedUser linkedUser)
        => new (
            linkedUser.DiscordId,
            linkedUser.Username,
            linkedUser.GlobalName,
            linkedUser.Description ?? string.Empty,
            linkedUser.AredlUserId,
            linkedUser.Country,
            linkedUser.CreatedInAredlAt);
    
    public static explicit operator PendingAredlLink(LinkedUser linkedUser)
        => new (
            linkedUser.DiscordId,
            linkedUser.Username,
            linkedUser.GlobalName,
            linkedUser.Description ?? string.Empty,
            linkedUser.AredlUserId,
            linkedUser.Country,
            linkedUser.CreatedInAredlAt);
}