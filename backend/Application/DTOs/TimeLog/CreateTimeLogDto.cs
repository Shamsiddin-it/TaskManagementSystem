namespace Application.DTOs;
public class CreateTimeLogDto
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public bool IsActive { get; set; }
}
