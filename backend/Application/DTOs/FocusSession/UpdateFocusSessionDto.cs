using Domain.Enums;

namespace Application.DTOs;

public class UpdateFocusSessionDto
{
    public int? TaskId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus? Status { get; set; }
    public string? Notes { get; set; }
}
