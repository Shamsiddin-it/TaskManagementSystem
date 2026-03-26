using System.Net;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

public class EmployerNotificationService : IEmployerNotificationService
{
    private readonly ApplicationDbContext _db;
    public EmployerNotificationService(ApplicationDbContext db) => _db = db;

    public async Task<Response<List<EmployerNotificationDto>>> GetNotificationsAsync(string employerId)
    {
        try
        {
            var result = await _db.EmployerNotifications
                .Where(n => n.EmployerId == employerId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new EmployerNotificationDto
                {
                    Id = n.Id,
                    Type = n.Type,
                    Priority = n.Priority,
                    Title = n.Title,
                    Body = n.Body,
                    ActionLabel = n.ActionLabel,
                    ActionUrl = n.ActionUrl,
                    SecondaryActionLabel = n.SecondaryActionLabel,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    RelatedProjectId = n.RelatedProjectId
                })
                .ToListAsync();

            return new Response<List<EmployerNotificationDto>>(
                HttpStatusCode.OK, "Notifications retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<List<EmployerNotificationDto>>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> MarkAsReadAsync(Guid notificationId)
    {
        try
        {
            var notif = await _db.EmployerNotifications.FindAsync(notificationId);
            if (notif == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Notification not found");

            notif.IsRead = true;
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Marked as read", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> MarkAllAsReadAsync(string employerId)
    {
        try
        {
            var notifs = await _db.EmployerNotifications
                .Where(n => n.EmployerId == employerId && !n.IsRead)
                .ToListAsync();

            notifs.ForEach(n => n.IsRead = true);
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, $"{notifs.Count} notifications marked as read", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> CreateNotificationAsync(
        string employerId,
        EmployerNotifType type,
        string title,
        string body,
        NotifPriority priority,
        Guid? projectId = null)
    {
        try
        {
            var notif = new EmployerNotification
            {
                EmployerId = employerId,
                Type = type,
                Priority = priority,
                Title = title,
                Body = body,
                RelatedProjectId = projectId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.EmployerNotifications.Add(notif);
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.Created, "Notification created successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}