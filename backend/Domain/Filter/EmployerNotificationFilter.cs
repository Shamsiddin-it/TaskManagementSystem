public class EmployerNotificationFilter
{
    public Guid? Id { get; set; }
    public Guid? EmployerId { get; set; }
    public Guid? RelatedProjectId { get; set; }
    public EmployerNotifType? Type { get; set; }
    public NotifPriority? Priority { get; set; }
    public string? Title { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
