/*
 * ⚠️ AVISO DE DEPRECIAÇÃO / DEPRECATION NOTICE ⚠️
 * -----------------------------------------------------------------------------
 * THIS SERVICE HAS BEEN DISCONTINUED AND SHOULD NO LONGER BE USED.
 * * REASON: 
 * The project underwent a complete architectural restructuring. For better
 * maintainability and separation of responsibilities, the functionalities of this
 * service were decomposed into 3 new parts within the Application layer.
 *
 * STATE: 
 * This file will be permanently REMOVED in the next stable release.
 * Please migrate the references to the corresponding new UseCases/LinkAredl.
 * -----------------------------------------------------------------------------
 */


using fixedhitbox.Application.Interfaces;
using fixedhitbox.Domain.Domain_DTOs;
using fixedhitbox.Domain.Entities;
using fixedhitbox.Domain.Enums;
using fixedhitbox.Infrastructure.Data;
using fixedhitbox.Infrastructure.External.Aredl.DTOs;
using fixedhitbox.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace fixedhitbox.z_Depreciation;

[Obsolete(
    "This service has been broken into 3 parts and will be removed in the next stable release. " +
    "Please utilize the new Application architecture.")]
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
    
    
    public async Task<ResultData<PendingAredlLink>> StartLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default)
    {

        if (cache.TryGetValue(LinkedUserKey(discordId), out PendingAredlLink? linkedUser)
            && linkedUser is not null)
            return ResultData<PendingAredlLink>.AlreadyLinked(linkedUser);
        
        if (cache.TryGetValue(PendingLinkKey(discordId), out PendingAredlLink? pendingLink)
            && pendingLink is not null)
            return ResultData<PendingAredlLink>.PendingConfirmation(pendingLink);
        
        if (cache.TryGetValue(NotFoundKey(discordId), out _))
            return ResultData<PendingAredlLink>.NotFound("Profile not found at aredl.net");
        
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var existingUser = await db.LinkedUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.DiscordId == discordId, cancellationToken);

        if (existingUser is not null)
        {
            var dto = PendingAredlLink.CreateFromEntity(existingUser);
            cache.Set(LinkedUserKey(discordId), dto, LinkedCacheOptions);
            
            return ResultData<PendingAredlLink>.AlreadyLinked(dto);
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
                    return ResultData<PendingAredlLink>.NotFound("Profile not found at aredl.net");
                
                case EAredlApiStatus.ConnectionError:
                    return ResultData<PendingAredlLink>.ConnectionError(
                        apiResult.ErrorMessage ?? "Connection failure with the aredl.net API.");
                
                case EAredlApiStatus.InvalidResponse:
                case EAredlApiStatus.UnexpectedError:
                    return ResultData<PendingAredlLink>.UnexpectedError(
                        apiResult.ErrorMessage ?? "An unexpected error occurred while processing the API response.");
                
                case EAredlApiStatus.Success:
                    //profile = apiResult.Data;
                    break;
                
                default:
                    return ResultData<PendingAredlLink>.UnexpectedError(
                        "aredl.net API sent an unexpected status code.");
            }
            
            // if (profile is null)
            //     return ResultData<PendingAredlLink>.UnexpectedError(
            //         "aredl.net API did not return a valid data");
            //
            // cache.Set(ProfileKey(discordId), profile, ProfileCacheOptions);
        }
        
        // var map = AredlProfileMapper.ToPendingLink(profile);
        //
        // if (!map.Success)
        //     return ResultData<PendingAredlLink>.ValidationError(map.Error!);
        //
        // cache.Remove(NotFoundKey(discordId));
        // cache.Set(PendingLinkKey(discordId), map.Value, PendingCacheOptions);
        //
        // return ResultData<PendingAredlLink>.PendingConfirmation(map.Value!);

        
        /*
         * This is only to satisfy the return of the method. */ return ResultData<PendingAredlLink>.NotFound();
        /* This service should no longer be used. It will be removed soon.
         */
    }

    public async Task<ResultData<PendingAredlLink>> ConfirmLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default)
    {

        if (!cache.TryGetValue(PendingLinkKey(discordId), out PendingAredlLink? pendingLink)
            || pendingLink is null)
            return ResultData<PendingAredlLink>.CacheExpired();
        
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var existingUser = await db.LinkedUsers
            .FirstOrDefaultAsync(user => user.DiscordId == discordId, cancellationToken);

        if (existingUser is not null)
        {
            var dto = PendingAredlLink.CreateFromEntity(existingUser);
            cache.Remove(PendingLinkKey(discordId));
            cache.Set(LinkedUserKey(discordId), dto, LinkedCacheOptions);
            
            return ResultData<PendingAredlLink>.AlreadyLinked(dto);
        }

        var entity = LinkedUser.CreateFromPending(pendingLink);

        try
        {
            db.LinkedUsers.Add(entity);
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ResultData<PendingAredlLink>.Conflict(
                "Couldn't complete the link because it already exists.");
        }

        cache.Remove(PendingLinkKey(discordId));
        cache.Remove(NotFoundKey(discordId));
        cache.Set(LinkedUserKey(discordId), pendingLink, LinkedCacheOptions);
        
        return ResultData<PendingAredlLink>.Success(pendingLink);
    }

    public async Task<ResultData<PendingAredlLink>> CancelLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(PendingLinkKey(discordId), out PendingAredlLink? pendingLink)
            && pendingLink is not null)
        {
            cache.Remove(PendingLinkKey(discordId));
            return ResultData<PendingAredlLink>.PendingConfirmation();
        }

        if (cache.TryGetValue(LinkedUserKey(discordId), out PendingAredlLink? linkedUser)
            && linkedUser is not null)
            return ResultData<PendingAredlLink>.AlreadyLinked();

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await db.LinkedUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.DiscordId == discordId, cancellationToken);

        if (user is not null)
        {
            var dto = PendingAredlLink.CreateFromEntity(user);
            cache.Set(LinkedUserKey(discordId), dto, LinkedCacheOptions);
            return ResultData<PendingAredlLink>.AlreadyLinked();
        }
        
        return ResultData<PendingAredlLink>.CacheExpired();
    }
}