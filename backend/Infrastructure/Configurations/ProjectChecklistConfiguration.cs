public class ProjectChecklistConfiguration : IEntityTypeConfiguration<ProjectChecklist>
{
    public void Configure(EntityTypeBuilder<ProjectChecklist> builder)
    {
        builder.ToTable("ProjectChecklists");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
