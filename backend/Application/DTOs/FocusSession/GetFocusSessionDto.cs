using Domain.Enums;

namespace Application.DTOs;

public class GetFocusSessionDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public Guid? TaskId { get; set; }
    public GetTaskDto? Task { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
