namespace Application.DTOs;

public class GetUserBadgeDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public int BadgeId { get; set; }
    public GetBadgeDto Badge { get; set; } = null!;
    public DateTime EarnedAt { get; set; }
}
