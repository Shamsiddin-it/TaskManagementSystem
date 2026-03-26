using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TeamId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.DevRole).IsRequired();
        builder.Property(x => x.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.WeeklyCapacityPct).HasDefaultValue(0);

        builder.HasIndex(x => new { x.TeamId, x.UserId }).IsUnique();

        builder.HasOne(x => x.Team)
            .WithMany()
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
