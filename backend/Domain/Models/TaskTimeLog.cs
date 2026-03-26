namespace Domain.Models;

using TaskEntity = Domain.Models.Task;

public class TaskTimeLog : BaseEntity
{
    public Guid TaskId { get; set; }
    public string UserId { get; set; } = null!;
    public int Minutes { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public TaskEntity? Task { get; set; }
    public ApplicationUser? User { get; set; }
}
