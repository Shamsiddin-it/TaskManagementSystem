using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserBadgeService(ApplicationDbContext dbContext) : IUserBadgeService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateUserBadgeDto dto)
    {
        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        if (!await context.Badges.AnyAsync(x => x.Id == dto.BadgeId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Badge not found");
        }

        var userBadge = new UserBadge
        {
            UserId = dto.UserId,
            BadgeId = dto.BadgeId,
            EarnedAt = DateTime.UtcNow
        };

        await context.UserBadges.AddAsync(userBadge);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add UserBadge successfully");
    }

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateUserBadgeDto dto)
    {
        var userBadge = await context.UserBadges.FindAsync(id);
        if (userBadge == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "UserBadge not found");
        }

        if (!string.IsNullOrWhiteSpace(dto.UserId))
        {
            if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
            {
                return new Response<string>(HttpStatusCode.NotFound, "User not found");
            }

            userBadge.UserId = dto.UserId;
        }

        if (dto.BadgeId.HasValue)
        {
            if (!await context.Badges.AnyAsync(x => x.Id == dto.BadgeId.Value))
            {
                return new Response<string>(HttpStatusCode.NotFound, "Badge not found");
            }

            userBadge.BadgeId = dto.BadgeId.Value;
        }

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update UserBadge successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var userBadge = await context.UserBadges.FindAsync(id);
        if (userBadge == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "UserBadge not found");
        }

        context.UserBadges.Remove(userBadge);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete UserBadge successfully");
    }

    public async Task<Response<List<GetUserBadgeDto>>> GetAllAsync()
    {
        var userBadges = await context.UserBadges
            .Include(x => x.User)
            .Include(x => x.Badge)
            .ToListAsync();

        var result = userBadges.Select(item => new GetUserBadgeDto
        {
            Id = item.Id,
            UserId = item.UserId,
            User = ServiceMappingHelper.ToGetUserDto(item.User),
            BadgeId = item.BadgeId,
            Badge = ServiceMappingHelper.ToGetBadgeDto(item.Badge),
            EarnedAt = item.EarnedAt
        }).ToList();

        return new Response<List<GetUserBadgeDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetUserBadgeDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Data?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetUserBadgeDto>(HttpStatusCode.NotFound, "UserBadge not found");
        }

        return new Response<GetUserBadgeDto>(HttpStatusCode.OK, "ok", item);
    }
}
