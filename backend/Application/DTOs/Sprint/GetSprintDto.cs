namespace Application.DTOs;

public class GetSprintDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public GetTeamDto Team { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
