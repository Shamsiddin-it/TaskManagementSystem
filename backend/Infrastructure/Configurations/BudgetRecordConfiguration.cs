public class BudgetRecordConfiguration : IEntityTypeConfiguration<BudgetRecord>
{
    public void Configure(EntityTypeBuilder<BudgetRecord> builder)
    {
        builder.ToTable("BudgetRecords");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
