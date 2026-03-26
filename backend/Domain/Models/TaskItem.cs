namespace Domain.Models;

public class TaskItem : BaseEntity
{
    // public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public bool IsBlocked { get; set; }
}
