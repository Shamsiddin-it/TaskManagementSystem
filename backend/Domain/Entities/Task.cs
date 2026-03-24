// Placeholder - full model from Team Lead
namespace Domain.Entities;
public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public int? AssignedTo { get; set; }
    public int? CreatedBy { get; set; }
    public string Status { get; set; } = "todo"; // todo | in_progress | review | done | blocked
    public string Priority { get; set; } = "medium"; // low | medium | high | critical
    public DateTime? Deadline { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public int? EstimatedHours { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
