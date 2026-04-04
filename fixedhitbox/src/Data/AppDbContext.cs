using fixedhitbox.Models;
using Microsoft.EntityFrameworkCore;

namespace fixedhitbox.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<LinkedUser> LinkedUsers => Set<LinkedUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<LinkedUser>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Property(entity => entity.LinkedAtUtc).CurrentValue = utcNow;
                entry.Property(entity => entity.LastUpdateAtUtc).CurrentValue = utcNow;
            }
            else if (entry.State is EntityState.Modified) 
                entry.Property(entity => entity.LastUpdateAtUtc).CurrentValue = utcNow;
        }
    }
}