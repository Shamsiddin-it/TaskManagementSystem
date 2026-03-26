namespace Application.DTOs;
public class CreateTeamMemberDto
{
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public string DevRole { get; set; } = "fullstack";
}
