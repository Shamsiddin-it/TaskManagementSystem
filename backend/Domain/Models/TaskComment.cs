namespace Domain.Models;

using TaskEntity = Domain.Models.Task;

public class TaskComment : BaseEntity
{
    public Guid TaskId { get; set; }
    public string AuthorId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public TaskEntity? Task { get; set; }
    public ApplicationUser? Author { get; set; }
}
