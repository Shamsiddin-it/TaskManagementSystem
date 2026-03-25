public class EmployerNotificationConfiguration : IEntityTypeConfiguration<EmployerNotification>
{
    public void Configure(EntityTypeBuilder<EmployerNotification> builder)
    {
        builder.ToTable("EmployerNotifications");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Priority).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Body).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.ActionLabel).HasMaxLength(100);
        builder.Property(x => x.ActionUrl).HasMaxLength(500);
        builder.Property(x => x.SecondaryActionLabel).HasMaxLength(100);

        builder.HasOne(x => x.Employer)
            .WithMany()
            .HasForeignKey(x => x.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RelatedProject)
            .WithMany()
            .HasForeignKey(x => x.RelatedProjectId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
