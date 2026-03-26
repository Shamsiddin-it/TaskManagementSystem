namespace Domain.Models;

public class MemberSkill : BaseEntity
{
    // public Guid Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string SkillName { get; set; } = string.Empty;    // "React", "TypeScript", "Docker"...
    public int OrderIndex { get; set; }
}
