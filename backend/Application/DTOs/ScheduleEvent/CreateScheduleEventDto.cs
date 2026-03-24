using Domain.Enums;

namespace Application.DTOs;

public class CreateScheduleEventDto
{
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? ColorHex { get; set; }
    public bool IsUrgent { get; set; }
}
