namespace Domain.Models;
public class UserBadge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public Guid BadgeId { get; set; }
    public DateTime EarnedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Badge Badge { get; set; } = null!;
}
