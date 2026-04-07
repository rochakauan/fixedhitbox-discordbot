using fixedhitbox.Application.DTOs;
using fixedhitbox.Application.Interfaces;
using fixedhitbox.Shared;
using fixedhitbox.Shared.Results.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace fixedhitbox.Infrastructure.Cache;

public sealed class AredlCache(IMemoryCache cache) : IAredlCache
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

    public ResultCacheData<PendingAredlLinkDto> TryGetAny(ulong discordId)
    {
        if (cache.TryGetValue(LinkedUserKey(discordId), out PendingAredlLinkDto? linked)
            && linked is not null)
            return ResultCacheData<PendingAredlLinkDto>.AlreadyLinked(linked);

        if (cache.TryGetValue(PendingLinkKey(discordId), out PendingAredlLinkDto? pending)
            && pending is not null)
            return ResultCacheData<PendingAredlLinkDto>.PendingConfirmation(pending);
        
        if (cache.TryGetValue(NotFoundKey(discordId), out _))
            return ResultCacheData<PendingAredlLinkDto>.NotFound(
                "Profile exists, and it is cached in NotFound key.");
        
        
        return ResultCacheData<PendingAredlLinkDto>.CacheExpired("There's nothing cached yet.");
    }

    public ResultCacheData<AredlProfileDto> GetProfile(ulong discordId)
    {
        if (cache.TryGetValue(ProfileKey(discordId), out AredlProfileDto? profile)
            && profile is not null)
            return ResultCacheData<AredlProfileDto>.ProfileFound(profile);

        return ResultCacheData<AredlProfileDto>.CacheExpired("This profile is not cached yet.");
        
    }

    public void SetProfile(ulong discordId, AredlProfileDto profile)
        => cache.Set(ProfileKey(discordId), profile, ProfileCacheOptions);
    
    public void SetPendingLink(ulong discordId, PendingAredlLinkDto pending)
    {
        RemoveNotFound(discordId);
        cache.Set(PendingLinkKey(discordId), pending, PendingCacheOptions);
    }
    
    public void SetLinkedUser(ulong discordId, PendingAredlLinkDto linked)
    {
        RemovePending(discordId);
        RemoveNotFound(discordId);
        cache.Set(LinkedUserKey(discordId), linked, LinkedCacheOptions);
    }
    
    public void SetNotFound(ulong discordId)
        => cache.Set(NotFoundKey(discordId), true, NotFoundCacheOptions);
    
    public void RemovePending(ulong discordId)
        => cache.Remove(PendingLinkKey(discordId));
    
    public void RemoveLinked(ulong discordId)
        => cache.Remove(LinkedUserKey(discordId));
    
    public void RemoveNotFound(ulong discordId)
        => cache.Remove(NotFoundKey(discordId));
    
}