public class EmployerNotificationDto
{
    public Guid Id { get; set; }
    public EmployerNotifType Type { get; set; }
    public NotifPriority Priority { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ActionLabel { get; set; }
    public string? ActionUrl { get; set; }
    public string? SecondaryActionLabel { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? RelatedProjectId { get; set; }
}
