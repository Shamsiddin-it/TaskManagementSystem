using Domain.Enums;

namespace Application.DTOs;

public class CreateTaskCommentDto
{
    public int TaskId { get; set; }
    public int AuthorId { get; set; }
    public string Message { get; set; } = string.Empty;
    public TaskCommentType Type { get; set; } = TaskCommentType.General;
}
