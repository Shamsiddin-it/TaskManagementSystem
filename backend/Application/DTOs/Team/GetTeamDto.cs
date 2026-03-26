namespace Application.DTOs;

public class GetTeamDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public GetProjectDto Project { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? TeamLeadId { get; set; }
    public GetUserDto? TeamLead { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
