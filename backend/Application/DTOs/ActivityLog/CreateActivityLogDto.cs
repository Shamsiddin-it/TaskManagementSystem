namespace Application.DTOs;

public class CreateActivityLogDto
{
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
