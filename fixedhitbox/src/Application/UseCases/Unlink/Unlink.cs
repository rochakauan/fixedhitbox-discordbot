using fixedhitbox.Application.DTOs;
using fixedhitbox.Application.Interfaces.Application.Aredl;
using fixedhitbox.Shared;

namespace fixedhitbox.Application.UseCases.Unlink;

internal sealed class Unlink : IUnlink
{
    public async Task<ResultData<AredlProfileDto>> UnlinkAsync(
        ulong discordId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}