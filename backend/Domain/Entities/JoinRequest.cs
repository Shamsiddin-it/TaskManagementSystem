using Domain.Enums;

namespace Domain.Entities;

public class JoinRequest
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public JoinRequestStatus Status { get; set; } = JoinRequestStatus.Pending;
    public string? CoverMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
}
