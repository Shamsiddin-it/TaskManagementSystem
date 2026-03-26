namespace Application.DTOs;

public class CreateTeamDto
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TeamLeadId { get; set; }
    public string? Description { get; set; }
}
