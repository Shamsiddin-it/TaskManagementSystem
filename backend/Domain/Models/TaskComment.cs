namespace Domain.Models;

using Domain.Enums;
using TaskEntity = Domain.Models.Task;

public class TaskComment : BaseEntity
{
    public Guid TaskId { get; set; }
    public string AuthorId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public TaskCommentType Type { get; set; } = TaskCommentType.General;
    public TaskEntity? Task { get; set; }
    public ApplicationUser? Author { get; set; }
}
