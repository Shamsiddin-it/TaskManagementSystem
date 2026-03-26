using Domain.Enums;

namespace Application.DTOs;

public class CreateFocusSessionDto
{
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus Status { get; set; } = FocusSessionStatus.Active;
    public string? Notes { get; set; }
}
