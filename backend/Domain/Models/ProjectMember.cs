namespace Domain.Models;

public class ProjectMember : BaseEntity
{
    // public int Id { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string ProjectRole { get; set; } = string.Empty;    // Frontend | Backend | Lead | Designer | QA
    public MemberAvailability Availability { get; set; }  // available | at_capacity | busy
    public DateTime JoinedAt { get; set; }
}
