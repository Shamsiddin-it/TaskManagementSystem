using Domain.Enums;

namespace Application.DTOs;

public class UpdateTaskCommentDto
{
    public string? Message { get; set; }
    public TaskCommentType? Type { get; set; }
}
