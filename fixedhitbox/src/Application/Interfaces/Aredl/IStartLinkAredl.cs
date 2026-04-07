using fixedhitbox.Application.DTOs;
using fixedhitbox.Shared;

namespace fixedhitbox.Application.Interfaces.Aredl;

public interface IStartLinkAredl
{
    Task<ResultData<PendingAredlLinkDto>> StartLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default);
}