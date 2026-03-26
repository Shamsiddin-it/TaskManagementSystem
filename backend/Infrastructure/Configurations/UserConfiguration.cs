using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.LastName).HasMaxLength(150);
        builder.Property(x => x.Email).HasMaxLength(150);
        builder.Property(x => x.UserName).HasMaxLength(150);
        builder.Property(x => x.AvatarUrl).HasMaxLength(500);
        builder.Property(x => x.AvatarInitials).HasMaxLength(10);
        builder.Property(x => x.AvatarColor).HasMaxLength(20);
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.OnlineStatus).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(x => x.Email).IsUnique();
    }
}
