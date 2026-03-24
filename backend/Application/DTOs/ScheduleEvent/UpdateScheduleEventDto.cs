using Domain.Enums;

namespace Application.DTOs;

public class UpdateScheduleEventDto
{
    public int? TaskId { get; set; }
    public string? Title { get; set; }
    public EventType? Type { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? ColorHex { get; set; }
    public bool? IsUrgent { get; set; }
}
