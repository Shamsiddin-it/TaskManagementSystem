namespace Application.DTOs;
public class CreateTeamMemberDto
{
    public Guid TeamId { get; set; }
    public string UserId { get; set; }
    public string DevRole { get; set; } = "fullstack";
}
