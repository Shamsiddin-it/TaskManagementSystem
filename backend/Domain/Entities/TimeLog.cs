namespace Domain.Entities;
public class TimeLog
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public bool IsActive { get; set; }

    public Task Task { get; set; } = null!;
    public User User { get; set; } = null!;
}
