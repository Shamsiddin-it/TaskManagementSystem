using Domain.Enums;

namespace Application.DTOs;

public class GetInvitationDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public GetTeamDto Team { get; set; } = null!;
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public int InvitedById { get; set; }
    public GetUserDto InvitedBy { get; set; } = null!;
    public InvitationStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
