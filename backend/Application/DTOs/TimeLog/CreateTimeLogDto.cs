namespace Application.DTOs;
public class CreateTimeLogDto
{
    public Guid TaskId { get; set; }
    public string UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public bool IsActive { get; set; }
}
