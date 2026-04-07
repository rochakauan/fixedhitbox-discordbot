using fixedhitbox.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fixedhitbox.Infrastructure.Data.Mappings;

public class LinkedUserMap : IEntityTypeConfiguration<LinkedUser>
{

    public void Configure(EntityTypeBuilder<LinkedUser> builder)
    {
        builder.ToTable("LinkedUsers");

        builder.HasKey(x => x.PrivateId);
        builder.Property(x => x.PrivateId)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.DiscordId)
            .IsRequired()
            .HasColumnName("discord_id");

        builder.Property(x => x.Username)
            .IsRequired()
            .HasColumnName("username")
            .HasMaxLength(100);

        builder.Property(x => x.GlobalName)
            .HasColumnName("global_name")
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(300);

        builder.Property(x => x.AredlUserId)
            .IsRequired()
            .HasColumnName("aredl_user_id");

        builder.Property(x => x.Country)
            .HasColumnName("country");
        
        builder.Property(x => x.CreatedInAredlAt)
            .HasColumnName("created_in_aredl_at_utc")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(x => x.LinkedAtUtc)
            .IsRequired()
            .HasColumnName("linked_at_utc")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.LastUpdateAtUtc)
            .IsRequired()
            .HasColumnName("last_update_at_utc")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(x => x.DiscordId)
            .IsUnique();
        builder.HasIndex(x => x.AredlUserId)
            .IsUnique();
    }
}