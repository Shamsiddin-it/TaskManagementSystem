using Domain.Enums;

namespace Application.DTOs;

public class CreateInvitationDto
{
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public int InvitedById { get; set; }
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public string? Message { get; set; }
    public DateTime ExpiresAt { get; set; }
}
