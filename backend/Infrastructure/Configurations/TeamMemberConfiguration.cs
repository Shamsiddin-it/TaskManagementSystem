using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.TeamId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.DevRole)
            .IsRequired();

        builder.Property(x => x.JoinedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.WeeklyCapacityPct)
            .HasDefaultValue(0);

        builder.HasIndex(x => new { x.TeamId, x.UserId })
            .IsUnique();

        builder.HasCheckConstraint("CK_TeamMembers_WeeklyCapacityPct_Range", "[WeeklyCapacityPct] >= 0 AND [WeeklyCapacityPct] <= 100");
        builder.HasCheckConstraint("CK_TeamMembers_FocusScore_Range", "[FocusScore] IS NULL OR ([FocusScore] >= 0 AND [FocusScore] <= 100)");

        builder.HasOne(x => x.Team)
            .WithMany()
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
