using Domain.Enums;

namespace Application.DTOs;

public class GetNotificationDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? RelatedId { get; set; }
    public string? RelatedType { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
