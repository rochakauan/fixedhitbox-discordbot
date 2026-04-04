using fixedhitbox.Dtos.Aredl;
using fixedhitbox.Services.Results;

namespace fixedhitbox.Services.Interfaces;

public interface ILinkAredlService
{
    Task<ResultData<PendingAredlLinkDto>> StartLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default);
    
    Task<ResultData<PendingAredlLinkDto>> ConfirmLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default);
    
    Task<ResultData<PendingAredlLinkDto>> CancelLinkAsync(
        ulong discordId, CancellationToken cancellationToken = default);
}