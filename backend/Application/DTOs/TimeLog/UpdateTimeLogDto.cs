namespace Application.DTOs;
public class UpdateTimeLogDto
{
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public bool? IsActive { get; set; }
}
