using Domain.Enums;

namespace Application.DTOs;

public class GetScheduleEventDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public Guid? TaskId { get; set; }
    public GetTaskDto? Task { get; set; }
    public string Title { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? ColorHex { get; set; }
    public bool IsUrgent { get; set; }
    public DateTime CreatedAt { get; set; }
}
