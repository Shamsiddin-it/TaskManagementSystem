public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255);
        builder.Property(x => x.AvatarUrl).HasMaxLength(500);
        builder.Property(x => x.AvatarInitials).HasMaxLength(10);
        builder.Property(x => x.AvatarColor).HasMaxLength(20);
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.OnlineStatus).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(x => x.Email).IsUnique();
    }
}
