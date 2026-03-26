public class ProjectTimelineConfiguration : IEntityTypeConfiguration<ProjectTimeline>
{
    public void Configure(EntityTypeBuilder<ProjectTimeline> builder)
    {
        builder.ToTable("ProjectTimelines");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PhaseName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.ColorHex).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
