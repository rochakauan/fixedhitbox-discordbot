using fixedhitbox.Data;
using fixedhitbox.Dtos.Aredl;
using fixedhitbox.Enums;
using fixedhitbox.Extensions;
using fixedhitbox.Models;
using fixedhitbox.Services.Interfaces;
using fixedhitbox.Services.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace fixedhitbox.Services;

public sealed class LinkAredlService(
    IDbContextFactory<AppDbContext> dbContextFactory,
    IAredlApiService aredlApiService,
    IMemoryCache cache) : ILinkAredlService
{

    private static readonly MemoryCacheEntryOptions PendingCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    };

    private static readonly MemoryCacheEntryOptions LinkedCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    private static readonly MemoryCacheEntryOptions ProfileCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
    };

    private static readonly MemoryCacheEntryOptions NotFoundCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };
    
    private static string PendingLinkKey(ulong discordId)
        => $"aredl:pending:{discordId}";
    
    private static string LinkedUserKey(ulong discordId)
        => $"aredl:linked:{discordId}";
    
    private static string ProfileKey(ulong discordId)
        => $"aredl:profile:{discordId}";
    
    private static string NotFoundKey(ulong discordId)
        => $"aredl:not-found:{discordId}";
    
    
    public async Task<ResultData<PendingAredlLinkDto>> StartLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default)
    {

        if (cache.TryGetValue(LinkedUserKey(discordId), out PendingAredlLinkDto? linkedUser)
            && linkedUser is not null)
            return ResultData<PendingAredlLinkDto>.AlreadyLinked(linkedUser);
        
        if (cache.TryGetValue(PendingLinkKey(discordId), out PendingAredlLinkDto? pendingLink)
            && pendingLink is not null)
            return ResultData<PendingAredlLinkDto>.PendingConfirmation(pendingLink);
        
        if (cache.TryGetValue(NotFoundKey(discordId), out _))
            return ResultData<PendingAredlLinkDto>.NotFound("Profile not found at aredl.net");
        
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var existingUser = await db.LinkedUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.DiscordId == discordId, cancellationToken);

        if (existingUser is not null)
        {
            var dto = existingUser.ToPendingDto();
            cache.Set(LinkedUserKey(discordId), dto, LinkedCacheOptions);
            
            return ResultData<PendingAredlLinkDto>.AlreadyLinked(dto);
        }

        AredlProfileResponse? profile;
        
        if (cache.TryGetValue(ProfileKey(discordId), out AredlProfileResponse? cachedProfile)
            && cachedProfile is not null)
            profile = cachedProfile;
        else
        {
            var apiResult = await aredlApiService.GetProfileAsync(discordId, cancellationToken);

            switch (apiResult.Status)
            {
                case EAredlApiStatus.NotFound:
                    cache.Set(NotFoundKey(discordId), true, NotFoundCacheOptions);
                    return ResultData<PendingAredlLinkDto>.NotFound("Profile not found at aredl.net");
                
                case EAredlApiStatus.ConnectionError:
                    return ResultData<PendingAredlLinkDto>.ConnectionError(
                        apiResult.ErrorMessage ?? "Connection failure with the aredl.net API.");
                
                case EAredlApiStatus.InvalidResponse:
                case EAredlApiStatus.UnexpectedError:
                    return ResultData<PendingAredlLinkDto>.UnexpectedError(
                        apiResult.ErrorMessage ?? "An unexpected error occurred while processing the API response.");
                
                case EAredlApiStatus.Success:
                    profile = apiResult.Data;
                    break;
                
                default:
                    return ResultData<PendingAredlLinkDto>.UnexpectedError(
                        "aredl.net API sent an unexpected status code.");
            }
            
            if (profile is null)
                return ResultData<PendingAredlLinkDto>.UnexpectedError(
                    "aredl.net API did not return a valid data");
            
            cache.Set(ProfileKey(discordId), profile, ProfileCacheOptions);
        }
        
        var pending = profile.ToPendingDto(discordId);
        cache.Remove(NotFoundKey(discordId));
        cache.Set(PendingLinkKey(discordId), pending, PendingCacheOptions);
        
        return ResultData<PendingAredlLinkDto>.PendingConfirmation(pending);
    }

    public async Task<ResultData<PendingAredlLinkDto>> ConfirmLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default)
    {

        if (!cache.TryGetValue(PendingLinkKey(discordId), out PendingAredlLinkDto? pendingLink)
            || pendingLink is null)
            return ResultData<PendingAredlLinkDto>.CacheExpired();
        
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var existingUser = await db.LinkedUsers
            .FirstOrDefaultAsync(user => user.DiscordId == discordId, cancellationToken);

        if (existingUser is not null)
        {
            var dto = existingUser.ToPendingDto();

            cache.Remove(PendingLinkKey(discordId));
            cache.Set(LinkedUserKey(discordId), dto, LinkedCacheOptions);
            
            return ResultData<PendingAredlLinkDto>.AlreadyLinked(dto);
        }

        var entity = new LinkedUser(
            pendingLink.DiscordId,
            pendingLink.Username,
            pendingLink.GlobalName,
            pendingLink.Description,
            pendingLink.AredlUserId,
            pendingLink.Country);

        try
        {
            db.LinkedUsers.Add(entity);
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ResultData<PendingAredlLinkDto>.Conflict(
                "Couldn't complete the link because it already exists.");
        }

        cache.Remove(PendingLinkKey(discordId));
        cache.Remove(NotFoundKey(discordId));
        cache.Set(LinkedUserKey(discordId), pendingLink, LinkedCacheOptions);
        
        return ResultData<PendingAredlLinkDto>.Success(pendingLink);
    }

    public async Task<ResultData<PendingAredlLinkDto>> CancelLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(PendingLinkKey(discordId), out PendingAredlLinkDto? pendingLink)
            && pendingLink is not null)
        {
            cache.Remove(PendingLinkKey(discordId));
            return ResultData<PendingAredlLinkDto>.PendingConfirmation();
        }

        if (cache.TryGetValue(LinkedUserKey(discordId), out PendingAredlLinkDto? linkedUser)
            && linkedUser is not null)
            return ResultData<PendingAredlLinkDto>.AlreadyLinked();

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await db.LinkedUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.DiscordId == discordId, cancellationToken);

        if (user is not null)
        {
            var dto = user.ToPendingDto();
            cache.Set(LinkedUserKey(discordId), dto, LinkedCacheOptions);
            return ResultData<PendingAredlLinkDto>.AlreadyLinked();
        }
        
        return ResultData<PendingAredlLinkDto>.CacheExpired();
    }
}