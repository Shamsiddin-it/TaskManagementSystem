namespace Application.DTOs;

public class GetTeamMemberDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public GetTeamDto Team { get; set; } = null!;
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public string DevRole { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}
