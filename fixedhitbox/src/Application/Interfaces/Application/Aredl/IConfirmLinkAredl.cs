using fixedhitbox.Application.DTOs;
using fixedhitbox.Shared;

namespace fixedhitbox.Application.Interfaces.Application;

public interface IConfirmLinkAredl
{
    Task<ResultData<PendingAredlLinkDto>> ConfirmLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default);
}