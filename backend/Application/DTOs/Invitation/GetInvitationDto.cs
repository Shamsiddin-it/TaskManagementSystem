using Domain.Enums;

namespace Application.DTOs;

public class GetInvitationDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public GetTeamDto Team { get; set; } = null!;
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public string InvitedById { get; set; }
    public GetUserDto InvitedBy { get; set; } = null!;
    public InvitationStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
