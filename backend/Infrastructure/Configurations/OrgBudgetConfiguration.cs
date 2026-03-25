public class OrgBudgetConfiguration : IEntityTypeConfiguration<OrgBudget>
{
    public void Configure(EntityTypeBuilder<OrgBudget> builder)
    {
        builder.ToTable("OrgBudgets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Period).IsRequired().HasMaxLength(100);
        builder.Property(x => x.TotalBudget).HasPrecision(18, 2);
        builder.Property(x => x.SpentAmount).HasPrecision(18, 2);
        builder.Property(x => x.BurnPercent).HasPrecision(18, 2);
        builder.Property(x => x.EstimatedRunwayMonths).HasPrecision(18, 2);

        builder.HasOne(x => x.Employer)
            .WithMany()
            .HasForeignKey(x => x.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.EmployerId).IsUnique();
    }
}
