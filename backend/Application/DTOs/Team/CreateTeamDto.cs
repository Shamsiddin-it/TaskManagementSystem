namespace Application.DTOs;

public class CreateTeamDto
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TeamLeadId { get; set; }
    public string? Description { get; set; }
}
