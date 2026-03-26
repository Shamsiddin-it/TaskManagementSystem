using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class NotificationService(ApplicationDbContext dbContext) : INotificationService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateNotificationDto dto)
    {
        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var notification = new Notification
        {
            UserId = dto.UserId,
            Type = dto.Type,
            Title = dto.Title,
            Body = dto.Body,
            RelatedId = dto.RelatedId,
            RelatedType = dto.RelatedType,
            IsRead = dto.IsRead,
            CreatedAt = DateTime.UtcNow
        };

        await context.Notifications.AddAsync(notification);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add Notification successfully");
    }

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateNotificationDto dto)
    {
        var notification = await context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Notification not found");
        }

        if (dto.Type.HasValue) notification.Type = dto.Type.Value;
        if (dto.Title != null) notification.Title = dto.Title;
        if (dto.Body != null) notification.Body = dto.Body;
        if (dto.RelatedId.HasValue) notification.RelatedId = dto.RelatedId;
        if (dto.RelatedType != null) notification.RelatedType = dto.RelatedType;
        if (dto.IsRead.HasValue) notification.IsRead = dto.IsRead.Value;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update Notification successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var notification = await context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Notification not found");
        }

        context.Notifications.Remove(notification);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete Notification successfully");
    }

    public async Task<Response<string>> MarkAsReadAsync(Guid id)
    {
        var notification = await context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Notification not found");
        }

        notification.IsRead = true;
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Notification marked as read successfully");
    }

    public async Task<Response<List<GetNotificationDto>>> GetAllAsync()
    {
        var notifications = await context.Notifications.Include(x => x.User).ToListAsync();
        var result = notifications.Select(notification => new GetNotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            User = ServiceMappingHelper.ToGetUserDto(notification.User),
            Type = notification.Type,
            Title = notification.Title,
            Body = notification.Body,
            RelatedId = notification.RelatedId,
            RelatedType = notification.RelatedType,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        }).ToList();

        return new Response<List<GetNotificationDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetNotificationDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Data?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetNotificationDto>(HttpStatusCode.NotFound, "Notification not found");
        }

        return new Response<GetNotificationDto>(HttpStatusCode.OK, "ok", item);
    }
}
