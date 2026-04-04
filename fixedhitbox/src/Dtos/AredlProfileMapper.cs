using fixedhitbox.Dtos.Aredl;

namespace fixedhitbox.Dtos;

public static class AredlProfileMapper
{
    public static bool TryNormalize(
        AredlProfileResponse dto,
        out PendingAredlLinkDto result,
        out string error)
    {
        if (dto.Username is null)
        {
            error = "Username is missing.";
            result = null!;
            return false;
        }

        if (dto.DiscordId is null)
        {
            error = "DiscordId is missing.";
            result = null!;
            return false;
        }

        if (dto.CreatedInAredlAt is null)
        {
            error = "CreatedAt is missing.";
            result = null!;
            return false;
        }

        result = PendingAredlLinkDto.Create(
            dto.DiscordId.Value,
            dto.Username,
            dto.GlobalName ?? string.Empty,
            dto.Description,
            dto.Id,
            dto.Country,
            dto.CreatedInAredlAt.Value); //Improve this later. Sleep time, please...TODO
        
        error = string.Empty;
        return true;
    }
}