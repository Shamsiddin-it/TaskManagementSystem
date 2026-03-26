namespace Application.DTOs;

public class CreateActivityLogDto
{
    public string UserId { get; set; }
    public Guid? TaskId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
