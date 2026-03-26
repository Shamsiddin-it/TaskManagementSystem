namespace Domain.Entities;
public class UserBadge
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BadgeId { get; set; }
    public DateTime EarnedAt { get; set; }

    public User User { get; set; } = null!;
    public Badge Badge { get; set; } = null!;
}
