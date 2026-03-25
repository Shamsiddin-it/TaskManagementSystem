public class MemberSkill
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string SkillName { get; set; } = string.Empty;    // "React", "TypeScript", "Docker"...
    public int OrderIndex { get; set; }
}
