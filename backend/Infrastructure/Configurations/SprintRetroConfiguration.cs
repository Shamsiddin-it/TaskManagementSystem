using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class SprintRetroConfiguration : IEntityTypeConfiguration<SprintRetro>
{
    public void Configure(EntityTypeBuilder<SprintRetro> builder)
    {
        builder.ToTable("SprintRetros");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.SprintId)
            .IsRequired();

        builder.Property(x => x.CreatedById)
            .IsRequired();

        builder.Property(x => x.PlannedPoints)
            .HasDefaultValue(0);

        builder.Property(x => x.CompletedPoints)
            .HasDefaultValue(0);

        builder.Property(x => x.SpilloverPoints)
            .HasDefaultValue(0);

        builder.HasIndex(x => x.SprintId)
            .IsUnique();

        builder.HasOne(x => x.Sprint)
            .WithOne(x => x.Retrospective)
            .HasForeignKey<SprintRetro>(x => x.SprintId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
