using Domain.Enums;

namespace Domain.Models;

public class JoinRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TeamId { get; set; }
    public string UserId { get; set; }
    public JoinRequestStatus Status { get; set; } = JoinRequestStatus.Pending;
    public string? CoverMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Team Team { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
