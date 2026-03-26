namespace Application.DTOs;

public class GetTimeLogDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public GetTaskDto Task { get; set; } = null!;
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public bool IsActive { get; set; }
}
