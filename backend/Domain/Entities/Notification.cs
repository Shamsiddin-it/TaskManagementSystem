using Domain.Enums;

namespace Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int? RelatedId { get; set; }
    public string? RelatedType { get; set; }  // task | team | invitation
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
