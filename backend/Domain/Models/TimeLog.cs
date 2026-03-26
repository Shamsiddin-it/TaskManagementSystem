namespace Domain.Models;
public class TimeLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TaskId { get; set; }
    public string UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public bool IsActive { get; set; }

    public Task Task { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
