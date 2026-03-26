using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserSettingsService(ApplicationDbContext dbContext) : IUserSettingsService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateUserSettingsDto dto)
    {
        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        if (await context.UserSettings.AnyAsync(x => x.UserId == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.BadRequest, "UserSettings already exists for this user");
        }

        var userSettings = new UserSettings
        {
            UserId = dto.UserId,
            AutoFocusEnabled = dto.AutoFocusEnabled,
            BlockNotificationsDuringFocus = dto.BlockNotificationsDuringFocus,
            DefaultFocusDurationMinutes = dto.DefaultFocusDurationMinutes,
            DailyGoalHours = dto.DailyGoalHours,
            UpdatedAt = DateTime.UtcNow
        };

        await context.UserSettings.AddAsync(userSettings);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add UserSettings successfully");
    }

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateUserSettingsDto dto)
    {
        var userSettings = await context.UserSettings.FindAsync(id);
        if (userSettings == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "UserSettings not found");
        }

        if (dto.AutoFocusEnabled.HasValue) userSettings.AutoFocusEnabled = dto.AutoFocusEnabled.Value;
        if (dto.BlockNotificationsDuringFocus.HasValue) userSettings.BlockNotificationsDuringFocus = dto.BlockNotificationsDuringFocus.Value;
        if (dto.DefaultFocusDurationMinutes.HasValue) userSettings.DefaultFocusDurationMinutes = dto.DefaultFocusDurationMinutes.Value;
        if (dto.DailyGoalHours.HasValue) userSettings.DailyGoalHours = dto.DailyGoalHours.Value;
        userSettings.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update UserSettings successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var userSettings = await context.UserSettings.FindAsync(id);
        if (userSettings == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "UserSettings not found");
        }

        context.UserSettings.Remove(userSettings);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete UserSettings successfully");
    }

    public async Task<Response<List<GetUserSettingsDto>>> GetAllAsync()
    {
        var settings = await context.UserSettings.Include(x => x.User).ToListAsync();
        var result = settings.Select(item => new GetUserSettingsDto
        {
            Id = item.Id,
            UserId = item.UserId,
            User = ServiceMappingHelper.ToGetUserDto(item.User),
            AutoFocusEnabled = item.AutoFocusEnabled,
            BlockNotificationsDuringFocus = item.BlockNotificationsDuringFocus,
            DefaultFocusDurationMinutes = item.DefaultFocusDurationMinutes,
            DailyGoalHours = item.DailyGoalHours,
            UpdatedAt = item.UpdatedAt
        }).ToList();

        return new Response<List<GetUserSettingsDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetUserSettingsDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Date?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetUserSettingsDto>(HttpStatusCode.NotFound, "UserSettings not found");
        }

        return new Response<GetUserSettingsDto>(HttpStatusCode.OK, "ok", item);
    }

    public async Task<Response<GetUserSettingsDto>> GetByUserIdAsync(string userId)
    {
        var all = await GetAllAsync();
        var item = all.Date?.FirstOrDefault(x => x.UserId == userId);
        if (item == null)
        {
            return new Response<GetUserSettingsDto>(HttpStatusCode.NotFound, "UserSettings not found");
        }

        return new Response<GetUserSettingsDto>(HttpStatusCode.OK, "ok", item);
    }
}
