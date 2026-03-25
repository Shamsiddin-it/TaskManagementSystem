public class WorkspaceSettingsConfiguration : IEntityTypeConfiguration<WorkspaceSettings>
{
    public void Configure(EntityTypeBuilder<WorkspaceSettings> builder)
    {
        builder.ToTable("WorkspaceSettings");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.EmployerId).IsUnique();
        builder.Property(x => x.OrganizationName).HasMaxLength(200);
        builder.Property(x => x.OrganizationCode).HasMaxLength(50);
        builder.Property(x => x.PrimaryContactName).HasMaxLength(150);
        builder.Property(x => x.ContactEmailAddress).HasMaxLength(200);
        builder.Property(x => x.CompanyWebsite).HasMaxLength(300);
        builder.Property(x => x.Industry).HasMaxLength(100);
        builder.Property(x => x.CompanySize).HasMaxLength(100);
        builder.Property(x => x.PlanName).HasMaxLength(50);
        builder.Property(x => x.PlanPriceMonthly).HasPrecision(18, 2);
        builder.Property(x => x.PaymentMethodLast4).HasMaxLength(4);
        builder.Property(x => x.BillingEmail).HasMaxLength(200);
        builder.Property(x => x.TaxIdOrVatNumber).HasMaxLength(100);
        builder.Property(x => x.DefaultPtoPolicy).HasMaxLength(100);
        builder.Property(x => x.DefaultWorkSchedule).HasMaxLength(100);
        builder.Property(x => x.PrimaryTimezone).HasMaxLength(100);
        builder.Property(x => x.IdleSessionTimeout).HasMaxLength(50);
        builder.Property(x => x.AuditLogRetention).HasMaxLength(50);
        builder.Property(x => x.SsoProviderName).HasMaxLength(100);
    }
}
