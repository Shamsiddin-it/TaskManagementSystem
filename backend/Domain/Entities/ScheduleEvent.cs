using Domain.Enums;

namespace Domain.Entities;

public class ScheduleEvent
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? ColorHex { get; set; }
    public bool IsUrgent { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Task? Task { get; set; }
}
