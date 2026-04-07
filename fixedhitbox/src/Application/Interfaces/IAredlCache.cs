using fixedhitbox.Application.DTOs;
using fixedhitbox.Shared;
using fixedhitbox.Shared.Results.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace fixedhitbox.Application.Interfaces;

public interface IAredlCache
{
    
    ResultCacheData<PendingAredlLinkDto> TryGetAny(ulong discordId);
    ResultCacheData<AredlProfileDto> GetProfile(ulong discordId);
    
    void SetProfile(ulong discordId, AredlProfileDto profile);
    void SetPendingLink(ulong discordId, PendingAredlLinkDto pending);
    void SetLinkedUser(ulong discordId, PendingAredlLinkDto linked);
    void SetNotFound(ulong discordId);
    
    void RemovePending(ulong discordId);
    void RemoveLinked(ulong discordId);
    void RemoveNotFound(ulong discordId);
    
}