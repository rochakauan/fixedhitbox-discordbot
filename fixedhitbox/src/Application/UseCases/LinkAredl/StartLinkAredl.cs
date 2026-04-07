using fixedhitbox.Application.DTOs;
using fixedhitbox.Application.Interfaces;
using fixedhitbox.Application.Interfaces.Aredl;
using fixedhitbox.Application.Mappers.Aredl;
using fixedhitbox.Domain.Enums;
using fixedhitbox.Domain.Repositories;
using fixedhitbox.Shared;
using fixedhitbox.Shared.Results.Cache;

namespace fixedhitbox.Application.UseCases.LinkAredl;

public sealed class StartLinkAredl(
    IUserRepository userRepository,
    IAredlApiService aredlApiService,
    IAredlCache aredlCache) : IStartLinkAredl
{
    public async Task<ResultData<PendingAredlLinkDto>> StartLinkAsync(
        ulong discordId, CancellationToken cancelationToken = default)
    {
        var cacheKeys = aredlCache.TryGetAny(discordId);
        var resultCache = ReturnCacheResultIfFound(cacheKeys, out var isSatisfied);
        
        switch (isSatisfied)
        {
            case false when resultCache.Status == EResultStatus.UnexpectedError:
            case true:
                return resultCache;
        }
        
        var existingUser = await userRepository
            .GetByDiscordIdAsync(discordId, cancelationToken);

        if (existingUser is not null)
        {
            var dto = AredlProfileMapper.MapFromEntity(existingUser);
            
            if (!dto.Success) return ResultData<PendingAredlLinkDto>
                .UnexpectedError(dto.Error ?? "AredlProfileMapper could not map a existing user.");
            
            aredlCache.SetLinkedUser(discordId, dto.Value!);
            return ResultData<PendingAredlLinkDto>.AlreadyLinked(dto.Value!);
        }

        var cachedProfile = aredlCache.GetProfile(discordId);
        
        if (cachedProfile.Status == ECacheResultStatus.ProfileFound)
        {
            var map = AredlProfileMapper.MapToPendingDto(cachedProfile.Data!);
            
            if (!map.Success)
                return ResultData<PendingAredlLinkDto>
                    .UnexpectedError(map.Error ?? "AredlProfileMapper could not map a profile.");
            
            return ResultData<PendingAredlLinkDto>.PendingConfirmation(map.Value!);
        }
        
        return await FetchProfileFromApi(discordId, cancelationToken);
    }

    private ResultData<PendingAredlLinkDto> ReturnCacheResultIfFound(
        ResultCacheData<PendingAredlLinkDto> cache,
        out bool isSatisfied)
    {
        if (cache.Status == ECacheResultStatus.CacheExpired)
        {
            isSatisfied = false;
            return ResultData<PendingAredlLinkDto>.CacheExpired();
        }
        
        switch (cache.Status)
        {
            case ECacheResultStatus.AlreadyLinked:
                isSatisfied = true;
                return ResultData<PendingAredlLinkDto>.AlreadyLinked(cache.Data!);
            
            case ECacheResultStatus.PendingConfirmation:
                isSatisfied = true;
                return ResultData<PendingAredlLinkDto>.PendingConfirmation(cache.Data!);
            
            case ECacheResultStatus.NotFound:
                isSatisfied = true;
                return ResultData<PendingAredlLinkDto>.NotFound();
            
            case ECacheResultStatus.UnexpectedError:
                isSatisfied = false;
                return ResultData<PendingAredlLinkDto>.UnexpectedError(
                    "Something went wrong with AredlCache service response.");
            default:
                isSatisfied = false;
                return default!;
        }
    }

    private async Task<ResultData<PendingAredlLinkDto>> FetchProfileFromApi
        (ulong discordId, CancellationToken cancellationToken = default)
    {
        var apiResult = await aredlApiService.GetProfileAsync(discordId, cancellationToken);

        switch (apiResult.Status)
        {
            case EAredlApiStatus.NotFound:
                aredlCache.SetNotFound(discordId);
                return ResultData<PendingAredlLinkDto>.NotFound("Profile not found at aredl.net.");
            
            case EAredlApiStatus.Timeout:
                return ResultData<PendingAredlLinkDto>.ConnectionError("AREDL API took too long to respond.");
            
            case EAredlApiStatus.ConnectionError:
                return ResultData<PendingAredlLinkDto>.ConnectionError(
                    apiResult.ErrorMessage);
            
            case EAredlApiStatus.InvalidResponse:
            case EAredlApiStatus.UnexpectedError:
                return ResultData<PendingAredlLinkDto>.UnexpectedError(
                    apiResult.ErrorMessage ?? "An unexpected error occurred while processing the API response.");

            case EAredlApiStatus.Success:
            {
                var resultPending = apiResult.Data;
                
                if (resultPending is null)
                    return ResultData<PendingAredlLinkDto>
                        .ValidationError("AredlProfileMapper returned a null data in IAredlApiService.");

                var map = AredlProfileMapper.MapFromPendingDto(resultPending);
                aredlCache.SetProfile(discordId, map.Value!);
                
                aredlCache.SetPendingLink(discordId, resultPending);
                return ResultData<PendingAredlLinkDto>.PendingConfirmation(resultPending);
            }
            
            default:
                return ResultData<PendingAredlLinkDto>.UnexpectedError(
                    apiResult.ErrorMessage ?? "Unhandled API response.");
        }
    }
}