using Domain.Enums;

namespace Domain.Entities;

public class TaskComment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int AuthorId { get; set; }
    public string Message { get; set; } = string.Empty;
    public TaskCommentType Type { get; set; } = TaskCommentType.General;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public User Author { get; set; } = null!;
}
