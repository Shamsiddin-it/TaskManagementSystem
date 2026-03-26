public class MemberSkillConfiguration : IEntityTypeConfiguration<MemberSkill>
{
    public void Configure(EntityTypeBuilder<MemberSkill> builder)
    {
        builder.ToTable("MemberSkills");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SkillName).IsRequired().HasMaxLength(100);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
