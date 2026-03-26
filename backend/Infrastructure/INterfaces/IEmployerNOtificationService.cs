public interface IEmployerNotificationService
{
    Task<Response<List<EmployerNotificationDto>>> GetNotificationsAsync(string employerId);
    Task<Response<bool>> MarkAsReadAsync(Guid notificationId);
    Task<Response<bool>> MarkAllAsReadAsync(string employerId);
    Task<Response<bool>> CreateNotificationAsync(string employerId, EmployerNotifType type, string title, string body, NotifPriority priority, Guid? projectId = null);
}