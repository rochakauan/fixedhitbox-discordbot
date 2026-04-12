using fixedhitbox.Domain.Entities;
using fixedhitbox.Shared.Results.Repository;

namespace fixedhitbox.Domain.Repositories;

public interface IUserRepository
{
    Task<LinkedUser?> GetByDiscordIdAsync(ulong discordId, CancellationToken cancellationToken = default);
    Task<LinkedUser?> GetByAredlIdAsync(Guid aredlId, CancellationToken cancellationToken = default);
    Task<LinkedUser?> GetByPrivateIdAsync(int id, CancellationToken cancellationToken = default);

    internal Task<RepositoryResult<LinkedUser>> Create(LinkedUser user, CancellationToken ct = default);
}