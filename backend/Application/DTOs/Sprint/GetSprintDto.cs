namespace Application.DTOs;

public class GetSprintDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public GetTeamDto Team { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
