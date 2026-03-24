namespace Application.DTOs;

public class GetTeamDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public GetProjectDto Project { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public int? TeamLeadId { get; set; }
    public GetUserDto? TeamLead { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
