public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.BudgetAllocated).HasPrecision(18, 2);
        builder.Property(x => x.BudgetSpent).HasPrecision(18, 2);

        builder.HasOne(x => x.Employer)
            .WithMany()
            .HasForeignKey(x => x.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
