using fixedhitbox.Domain.Entities;
using fixedhitbox.Domain.Enums;
using fixedhitbox.Domain.Repositories;
using fixedhitbox.Shared.Results.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace fixedhitbox.Infrastructure.Data.Repositories;

public sealed class UserRepository(
    IDbContextFactory<AppDbContext> factory,
    ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<LinkedUser?> GetByDiscordIdAsync(ulong discordId, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        try
        {
            return await db.LinkedUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DiscordId == discordId, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while trying to read information from a database table.");
            return null;
        }
    }
    public async Task<LinkedUser?> GetByAredlIdAsync(Guid aredlId, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        return await db.LinkedUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AredlUserId == aredlId, ct);
    }

    public async Task<LinkedUser?> GetByPrivateIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        return await db.LinkedUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PrivateId == id, ct);
    }

    public async Task<RepositoryResult<LinkedUser>> Create(LinkedUser user, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        try
        {
            db.LinkedUsers.Add(user);
            await db.SaveChangesAsync(ct);
            return RepositoryResult<LinkedUser>.Ok(user);
        }
        catch (DbUpdateException ex)
        {
            return RepositoryResult<LinkedUser>.Fail(
                ERepositoryStatus.Conflict,
                ex.Message
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while persisting user data in the database.");
            return RepositoryResult<LinkedUser>.Fail(
                ERepositoryStatus.UnexpectedError,
                ex.Message
            );
        }
    }
}