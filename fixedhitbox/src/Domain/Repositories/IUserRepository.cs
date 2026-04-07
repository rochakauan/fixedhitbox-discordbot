using fixedhitbox.Domain.Entities;

namespace fixedhitbox.Domain.Repositories;

public interface IUserRepository
{
    Task<LinkedUser?> GetByDiscordIdAsync(ulong discordId, CancellationToken cancellationToken = default);
    Task<LinkedUser?> GetByAredlIdAsync(Guid aredlId, CancellationToken cancellationToken = default);
    Task<LinkedUser?> GetByPrivateIdAsync(int id, CancellationToken cancellationToken = default);
}