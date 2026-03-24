using Domain.Enums;

namespace Application.DTOs;

public class GetJoinRequestDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public GetTeamDto Team { get; set; } = null!;
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public JoinRequestStatus Status { get; set; }
    public string? CoverMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}
