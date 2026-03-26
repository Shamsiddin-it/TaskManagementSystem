public class InsertTaskDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int TeamId { get; set; }
    public string AssignedToId { get; set; } = null!;
    public string CreatedById { get; set; } = null!;
    public int? SprintId { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TicketType TicketType { get; set; } = TicketType.Task;
    public DateTime? Deadline { get; set; }
    public int? EstimatedHours { get; set; }
    public int? StoryPoints { get; set; }
}

public class UpdateTaskDto : InsertTaskDto
{
    public int Id { get; set; }
}

public class GetTaskDto
{
    public int Id { get; set; }
    public string TicketCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int TeamId { get; set; }
    public string AssignedToId { get; set; } = null!;
    public string CreatedById { get; set; } = null!;
    public int? SprintId { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public TicketType TicketType { get; set; }
    public DateTime? Deadline { get; set; }
    public int? EstimatedHours { get; set; }
    public int? StoryPoints { get; set; }
    public int OrderIndex { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockedReason { get; set; }
    public int TotalTimeMinutes { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
