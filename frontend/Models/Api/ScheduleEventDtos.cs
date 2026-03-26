namespace TaskManager.Web.Models.Api;

public enum EventType
{
    FocusBlock = 0,
    Meeting = 1,
    Task = 2,
    Personal = 3
}

public sealed class GetScheduleEventDto
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
}

public sealed class CreateScheduleEventDto
{
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public EventType Type { get; set; } = EventType.Task;
    public DateTime StartTime { get; set; } = DateTime.Today.AddHours(9);
    public DateTime EndTime { get; set; } = DateTime.Today.AddHours(10);
    public string? ColorHex { get; set; }
    public bool IsUrgent { get; set; }
}

