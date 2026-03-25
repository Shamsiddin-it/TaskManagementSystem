public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string ProjectRole { get; set; } = string.Empty;    // Frontend | Backend | Lead | Designer | QA
    public MemberAvailability Availability { get; set; }  // available | at_capacity | busy
    public DateTime JoinedAt { get; set; }
}
