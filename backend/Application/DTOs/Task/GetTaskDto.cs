namespace Application.DTOs;

public class GetTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public GetTeamDto Team { get; set; } = null!;
    public string? AssignedTo { get; set; }
    public GetUserDto? AssignedToUser { get; set; }
    public string? CreatedBy { get; set; }
    public GetUserDto? CreatedByUser { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public int? EstimatedHours { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
