
namespace Application.DTOs;

public class GetActivityLogDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public Guid? TaskId { get; set; }
    public GetTaskDto? Task { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
