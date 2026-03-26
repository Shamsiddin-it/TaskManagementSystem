using Domain.Enums;

namespace Domain.Models;

public class FocusSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public Guid? TaskId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus Status { get; set; } = FocusSessionStatus.Active;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
