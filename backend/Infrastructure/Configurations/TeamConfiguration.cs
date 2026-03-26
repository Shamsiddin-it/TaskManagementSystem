public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Teams)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TeamLead)
            .WithMany()
            .HasForeignKey(x => x.TeamLeadId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
