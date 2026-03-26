namespace Application.DTOs;

public class GetUserBadgeDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public Guid BadgeId { get; set; }
    public GetBadgeDto Badge { get; set; } = null!;
    public DateTime EarnedAt { get; set; }
}
