using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;

namespace Infrastructure.Persistence.Configurations;

public sealed class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.ToTable("Sprints", table =>
            table.HasCheckConstraint("CK_Sprints_EndDateAfterStartDate", "\"EndDate\" > \"StartDate\""));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.TeamId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Number)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasDefaultValue(SprintStatus.Planning)
            .HasSentinel((SprintStatus)0);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.TotalPoints)
            .HasDefaultValue(0);

        builder.Property(x => x.CompletedPoints)
            .HasDefaultValue(0);

        builder.Property(x => x.CapacityPoints)
            .HasDefaultValue(40);

        builder.HasIndex(x => new { x.TeamId, x.Number })
            .IsUnique();

        builder.HasOne(x => x.Team)
            .WithMany()
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Tasks)
            .WithOne(x => x.Sprint)
            .HasForeignKey(x => x.SprintId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Retrospective)
            .WithOne(x => x.Sprint)
            .HasForeignKey<SprintRetro>(x => x.SprintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
