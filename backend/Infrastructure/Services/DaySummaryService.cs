using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class DaySummaryService(ApplicationDbContext dbContext) : IDaySummaryService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateDaySummaryDto dto)
    {
        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var daySummary = new DaySummary
        {
            UserId = dto.UserId,
            SummaryDate = dto.SummaryDate,
            TasksCompleted = dto.TasksCompleted,
            TasksTotal = dto.TasksTotal,
            FocusHours = dto.FocusHours,
            ProductivityScore = dto.ProductivityScore,
            ProductivityGrade = dto.ProductivityGrade,
            StreakDays = dto.StreakDays,
            TomorrowPriority1 = dto.TomorrowPriority1,
            TomorrowPriority2 = dto.TomorrowPriority2,
            TomorrowPriority3 = dto.TomorrowPriority3,
            CreatedAt = DateTime.UtcNow
        };

        await context.DaySummaries.AddAsync(daySummary);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add DaySummary successfully");
    }

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateDaySummaryDto dto)
    {
        var daySummary = await context.DaySummaries.FindAsync(id);
        if (daySummary == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "DaySummary not found");
        }

        if (dto.SummaryDate.HasValue) daySummary.SummaryDate = dto.SummaryDate.Value;
        if (dto.TasksCompleted.HasValue) daySummary.TasksCompleted = dto.TasksCompleted.Value;
        if (dto.TasksTotal.HasValue) daySummary.TasksTotal = dto.TasksTotal.Value;
        if (dto.FocusHours.HasValue) daySummary.FocusHours = dto.FocusHours.Value;
        if (dto.ProductivityScore.HasValue) daySummary.ProductivityScore = dto.ProductivityScore.Value;
        if (dto.ProductivityGrade != null) daySummary.ProductivityGrade = dto.ProductivityGrade;
        if (dto.StreakDays.HasValue) daySummary.StreakDays = dto.StreakDays.Value;
        if (dto.TomorrowPriority1 != null) daySummary.TomorrowPriority1 = dto.TomorrowPriority1;
        if (dto.TomorrowPriority2 != null) daySummary.TomorrowPriority2 = dto.TomorrowPriority2;
        if (dto.TomorrowPriority3 != null) daySummary.TomorrowPriority3 = dto.TomorrowPriority3;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update DaySummary successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var daySummary = await context.DaySummaries.FindAsync(id);
        if (daySummary == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "DaySummary not found");
        }

        context.DaySummaries.Remove(daySummary);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete DaySummary successfully");
    }

    public async Task<Response<List<GetDaySummaryDto>>> GetAllAsync()
    {
        var summaries = await context.DaySummaries.Include(x => x.User).ToListAsync();
        var result = summaries.Select(summary => new GetDaySummaryDto
        {
            Id = summary.Id,
            UserId = summary.UserId,
            User = ServiceMappingHelper.ToGetUserDto(summary.User),
            SummaryDate = summary.SummaryDate,
            TasksCompleted = summary.TasksCompleted,
            TasksTotal = summary.TasksTotal,
            FocusHours = summary.FocusHours,
            ProductivityScore = summary.ProductivityScore,
            ProductivityGrade = summary.ProductivityGrade,
            StreakDays = summary.StreakDays,
            TomorrowPriority1 = summary.TomorrowPriority1,
            TomorrowPriority2 = summary.TomorrowPriority2,
            TomorrowPriority3 = summary.TomorrowPriority3,
            CreatedAt = summary.CreatedAt
        }).ToList();

        return new Response<List<GetDaySummaryDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetDaySummaryDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Data?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetDaySummaryDto>(HttpStatusCode.NotFound, "DaySummary not found");
        }

        return new Response<GetDaySummaryDto>(HttpStatusCode.OK, "ok", item);
    }
}
