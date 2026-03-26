namespace Domain.Models;

public class TeamMember : BaseEntity
{
    public Guid TeamId { get; set; }
    public string UserId { get; set; } = null!;
    public DevRole DevRole { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public int WeeklyCapacityPct { get; set; } = 0;
    public int? FocusScore { get; set; }
    public decimal? ThroughputPtsPerWk { get; set; }

    public Team? Team { get; set; }
    public ApplicationUser? User { get; set; }
    public List<Task>? AssignedTasks { get; set; }
}
