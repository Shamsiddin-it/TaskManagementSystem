using Domain.Enums;

namespace Domain.Entities;

public class Invitation
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }       // кому приглашение
    public int InvitedById { get; set; }  // Team Lead или Employer
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public string? Message { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
}
