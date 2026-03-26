using Domain.Enums;

namespace Application.DTOs;

public class UpdateNotificationDto
{
    public NotificationType? Type { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public Guid? RelatedId { get; set; }
    public string? RelatedType { get; set; }
    public bool? IsRead { get; set; }
}
