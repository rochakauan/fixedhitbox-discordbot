using fixedhitbox.Application.DTOs;
using fixedhitbox.Application.Interfaces.Application;
using fixedhitbox.Application.Interfaces.Application.Aredl;
using fixedhitbox.Application.Interfaces.Infra.Aredl;
using fixedhitbox.Application.Mappers.Aredl;
using fixedhitbox.Domain.Entities;
using fixedhitbox.Domain.Enums;
using fixedhitbox.Domain.Repositories;
using fixedhitbox.Shared;
using fixedhitbox.Shared.Results.Cache;
using Microsoft.Extensions.Logging;

namespace fixedhitbox.Application.UseCases.LinkAredl;

public sealed class ConfirmLinkAredl(
    IAredlCache aredlCache,
    IUserRepository userRepository,
    ILogger<ConfirmLinkAredl> logger) : IConfirmLinkAredl
{

    private readonly string _errorPrefix = "[ConfirmLinkAredl]";
    private string _errorMessage = "(!) Nothing specified.";

    public async Task<ResultData<PendingAredlLinkDto>> ConfirmLinkAsync(
        ulong discordId,
        CancellationToken cancellationToken = default)
    {
        var cacheKeys = aredlCache.TryGetAny(discordId);
        var resultCache = ReturnCacheResultIfFound(cacheKeys, out var isSatisfied);

        switch (isSatisfied)
        {
            // case false when resultCache.Status == EResultStatus.UnexpectedError:
            case false when resultCache.Status == EResultStatus.CacheExpired:
            case true when resultCache.Status == EResultStatus.AlreadyLinked:
                return resultCache;
        }

        var existingUser = await userRepository
            .GetByDiscordIdAsync(discordId, cancellationToken);

        if (existingUser is not null)
        {
            var dto = AredlProfileMapper.MapFromEntity(existingUser);

            if (!dto.Success)
            {
                _errorMessage = "AredlProfileMapper could not map an existing user.";

                logger.LogError(_errorMessage);
                return ResultData<PendingAredlLinkDto>
                .UnexpectedError(dto.Error ??
                $"{_errorPrefix} {_errorMessage}");
            }

            aredlCache.SetLinkedUser(discordId, dto.Value!);
            return ResultData<PendingAredlLinkDto>
                .AlreadyLinked(dto.Value!);
        }

        if (resultCache.Data is null)
        {
            _errorMessage = "An unexpected error occured when attempting to handle the data from AredlCache service." +
            "This may have happened because there is a possibility that StartLinkAredl broke the execution flow.";

            logger.LogError(_errorMessage);
            return ResultData<PendingAredlLinkDto>
                .UnexpectedError(resultCache.ErrorMessage ??
                $"{_errorPrefix} {_errorMessage}");
        }

        var entity = new LinkedUser(
            resultCache.Data.DiscordId,
            resultCache.Data.Username,
            resultCache.Data.GlobalName,
            resultCache.Data.Description,
            resultCache.Data.Id,
            resultCache.Data.Country
        );

        var linkedUser = await userRepository.Create(entity, cancellationToken);

        if (!linkedUser.Success)
        {
            _errorMessage = "UserRepository failed to persist the data in LinkedUser table.";

            logger.LogWarning("The execution flow was interrupted by an external exception.");
            return ResultData<PendingAredlLinkDto>
            .Conflict(linkedUser.Error ??
            $"{_errorPrefix} {_errorMessage}");

        }
        aredlCache.SetLinkedUser(discordId, resultCache.Data);

        logger.LogInformation(
            "{Username} ({DiscordId}) has successfully linked!",
            resultCache.Data.Username, discordId);

        return ResultData<PendingAredlLinkDto>.Success(resultCache.Data);
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

            // case ECacheResultStatus.UnexpectedError:
            //     isSatisfied = false;
            //     return ResultData<PendingAredlLinkDto>.UnexpectedError(
            //         $"{_errorPrefix} Something went wrong with AredlCache service response.");
            default:
                isSatisfied = false;
                return default!;
        }
    }

}