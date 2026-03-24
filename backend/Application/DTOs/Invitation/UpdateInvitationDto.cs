using Domain.Enums;

namespace Application.DTOs;

public class UpdateInvitationDto
{
    public InvitationStatus? Status { get; set; }
    public string? Message { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
