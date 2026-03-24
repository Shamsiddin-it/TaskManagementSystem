namespace Application.DTOs;

public class GetTimeLogDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public GetTaskDto Task { get; set; } = null!;
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public bool IsActive { get; set; }
}
