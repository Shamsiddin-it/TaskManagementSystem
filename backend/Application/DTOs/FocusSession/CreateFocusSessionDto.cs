using Domain.Enums;

namespace Application.DTOs;

public class CreateFocusSessionDto
{
    public string UserId { get; set; }
    public Guid? TaskId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus Status { get; set; } = FocusSessionStatus.Active;
    public string? Notes { get; set; }
}
