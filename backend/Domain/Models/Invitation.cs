using Domain.Enums;

namespace Domain.Models;

public class Invitation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TeamId { get; set; }
    public string UserId { get; set; }       // кому приглашение
    public string InvitedById { get; set; }  // Team Lead или Employer
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public string? Message { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Team Team { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
