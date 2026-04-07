using fixedhitbox.Application.DTOs;
using fixedhitbox.Shared.Results.Api;

namespace fixedhitbox.Application.Interfaces;

public interface IAredlApiService
{

    Task<AredlApiResult<PendingAredlLinkDto>> GetProfileAsync(
        ulong discordId,
        CancellationToken cancellationToken);
}