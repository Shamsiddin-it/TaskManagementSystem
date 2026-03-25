using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class RetroActionItemConfiguration : IEntityTypeConfiguration<RetroActionItem>
{
    public void Configure(EntityTypeBuilder<RetroActionItem> builder)
    {
        builder.ToTable("RetroActionItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.RetroId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Priority)
            .HasDefaultValue(ActionItemPriority.Medium);

        builder.Property(x => x.IsDone)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.RetroId);
        builder.HasIndex(x => x.AssignedToId);

        builder.HasOne(x => x.Retro)
            .WithMany(x => x.ActionItems)
            .HasForeignKey(x => x.RetroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AssignedTo)
            .WithMany()
            .HasForeignKey(x => x.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
