namespace Domain.Models;

using TaskEntity = Domain.Models.Task;

public class TaskComment : BaseEntity
{
    public Guid TaskId { get; set; }
    public string UserId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public TaskEntity? Task { get; set; }
    public ApplicationUser? User { get; set; }
}
