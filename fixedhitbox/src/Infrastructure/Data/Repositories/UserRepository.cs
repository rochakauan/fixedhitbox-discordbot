using fixedhitbox.Domain.Entities;
using fixedhitbox.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace fixedhitbox.Infrastructure.Data.Repositories;

public sealed class UserRepository(IDbContextFactory<AppDbContext> factory) : IUserRepository
{
    public async Task<LinkedUser?> GetByDiscordIdAsync(ulong discordId, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        
        return await db.LinkedUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DiscordId == discordId, ct);
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
}