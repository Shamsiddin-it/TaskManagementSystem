using Domain.Enums;

namespace Application.DTOs;

public class CreateInvitationDto
{
    public Guid TeamId { get; set; }
    public string UserId { get; set; }
    public string InvitedById { get; set; }
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public string? Message { get; set; }
    public DateTime ExpiresAt { get; set; }
}
