using Domain.Enums;

namespace Domain.Entities;

public class FocusSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus Status { get; set; } = FocusSessionStatus.Active;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
