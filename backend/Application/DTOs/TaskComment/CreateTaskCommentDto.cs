using Domain.Enums;

namespace Application.DTOs;

public class CreateTaskCommentDto
{
    public Guid TaskId { get; set; }
    public string AuthorId { get; set; }
    public string Message { get; set; } = string.Empty;
    public TaskCommentType Type { get; set; } = TaskCommentType.General;
}
