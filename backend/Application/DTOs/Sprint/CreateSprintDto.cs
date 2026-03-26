namespace Application.DTOs;

public class CreateSprintDto
{
    public Guid TeamId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "active";
}
