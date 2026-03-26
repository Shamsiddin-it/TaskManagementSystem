using Domain.Enums;

namespace Application.DTOs;

public class GetTaskCommentDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public GetTaskDto Task { get; set; } = null!;
    public int AuthorId { get; set; }
    public GetUserDto Author { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
    public TaskCommentType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
