namespace Application.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public string? AssignedTo { get; set; }
    public string? AssignedToId { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedById { get; set; }
    public string Status { get; set; } = "todo";
    public string Priority { get; set; } = "medium";
    public string TicketType { get; set; } = "Task";
    public Guid? SprintId { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public int? EstimatedHours { get; set; }
    public int? StoryPoints { get; set; }
    public int OrderIndex { get; set; }
}
