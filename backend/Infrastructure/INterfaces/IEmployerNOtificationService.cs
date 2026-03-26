public interface IEmployerNotificationService
{
    Task<Response<List<EmployerNotificationDto>>> GetNotificationsAsync(Guid employerId);
    Task<Response<bool>> MarkAsReadAsync(Guid notificationId);
    Task<Response<bool>> MarkAllAsReadAsync(Guid employerId);
    Task<Response<bool>> CreateNotificationAsync(Guid employerId, EmployerNotifType type, string title, string body, NotifPriority priority, Guid? relatedProjectId = null);
}