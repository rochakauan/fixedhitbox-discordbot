using fixedhitbox.Application.DTOs;
using fixedhitbox.Shared;

namespace fixedhitbox.Application.Interfaces.Application.Aredl;

internal interface IUnlink
{
    internal Task<ResultData<AredlProfileDto>> UnlinkAsync(
        ulong discordId, CancellationToken cancellationToken = default);
}