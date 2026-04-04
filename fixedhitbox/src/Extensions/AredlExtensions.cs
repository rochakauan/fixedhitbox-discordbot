using fixedhitbox.Dtos.Aredl;
using fixedhitbox.Models;

namespace fixedhitbox.Extensions;

public static class AredlExtensions
{

    public static PendingAredlLinkDto ToPendingDto(this LinkedUser entity)
        => new()
        {
            DiscordId = entity.DiscordId,
            Username = entity.Username,
            GlobalName = entity.GlobalName,
            Description = entity.Description,
            AredlUserId = entity.AredlUserId,
            Country = entity.Country
        };
    
    public static PendingAredlLinkDto ToPendingDto(
        this AredlProfileResponse profile, ulong discordId)
        => new()
        {
            DiscordId = discordId,
            Username = profile.Username,
            GlobalName = profile.GlobalName,
            Description = profile.Description,
            AredlUserId = profile.Id,
            Country = profile.Country
        };
}