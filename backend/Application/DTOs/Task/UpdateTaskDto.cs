namespace Application.DTOs;
public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? AssignedTo { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public int? EstimatedHours { get; set; }
    public int? OrderIndex { get; set; }
}
