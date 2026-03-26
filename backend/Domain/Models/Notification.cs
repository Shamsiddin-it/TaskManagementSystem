using Domain.Enums;

namespace Domain.Models;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? RelatedId { get; set; }
    public string? RelatedType { get; set; }  // task | team | invitation
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public ApplicationUser User { get; set; } = null!;
}
