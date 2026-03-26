using Domain.Models;

public class EmployerNotification : BaseEntity
{
    // public int Id { get; set; }
    public string EmployerId { get; set; }
    public ApplicationUser Employer { get; set; } = null!;
    public EmployerNotifType Type { get; set; }
    // deadline_warning | budget_alert | team_update | milestone_reached | system | access_request
    public NotifPriority Priority { get; set; }   // high | normal | low
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int? RelatedProjectId { get; set; }
    public Project? RelatedProject { get; set; }
    public string? ActionLabel { get; set; }    // "Action", "View Detail", "Approve"
    public string? ActionUrl { get; set; }
    public string? SecondaryActionLabel { get; set; }  // "Deny"
    public bool IsRead { get; set; }
    // public DateTime CreatedAt { get; set; }
}
