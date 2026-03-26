namespace Application.DTOs;

public class GetTeamMemberDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public GetTeamDto Team { get; set; } = null!;
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public string DevRole { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}
