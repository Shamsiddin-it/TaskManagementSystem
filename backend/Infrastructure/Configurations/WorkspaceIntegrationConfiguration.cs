public class WorkspaceIntegrationConfiguration : IEntityTypeConfiguration<WorkspaceIntegration>
{
    public void Configure(EntityTypeBuilder<WorkspaceIntegration> builder)
    {
        builder.ToTable("WorkspaceIntegrations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.EmployerId, x.Key }).IsUnique();
        builder.Property(x => x.Key).HasMaxLength(50);
        builder.Property(x => x.Name).HasMaxLength(150);
        builder.Property(x => x.Status).HasMaxLength(50);
        builder.Property(x => x.Accent).HasMaxLength(50);
    }
}
