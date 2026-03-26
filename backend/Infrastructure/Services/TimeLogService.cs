using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TimeLogService(ApplicationDbContext dbContext) : ITimeLogService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateTimeLogDto dto)
    {
        if (!await context.Tasks.AnyAsync(x => x.Id == dto.TaskId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Task not found");
        }

        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var timeLog = new TimeLog
        {
            TaskId = dto.TaskId,
            UserId = dto.UserId,
            StartedAt = dto.StartedAt,
            EndedAt = dto.EndedAt,
            DurationMinutes = dto.DurationMinutes,
            IsActive = dto.IsActive
        };

        await context.TimeLogs.AddAsync(timeLog);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add TimeLog successfully");
    }

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateTimeLogDto dto)
    {
        var timeLog = await context.TimeLogs.FindAsync(id);
        if (timeLog == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "TimeLog not found");
        }

        if (dto.StartedAt.HasValue) timeLog.StartedAt = dto.StartedAt.Value;
        if (dto.EndedAt.HasValue) timeLog.EndedAt = dto.EndedAt.Value;
        if (dto.DurationMinutes.HasValue) timeLog.DurationMinutes = dto.DurationMinutes.Value;
        if (dto.IsActive.HasValue) timeLog.IsActive = dto.IsActive.Value;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update TimeLog successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var timeLog = await context.TimeLogs.FindAsync(id);
        if (timeLog == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "TimeLog not found");
        }

        context.TimeLogs.Remove(timeLog);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete TimeLog successfully");
    }

    public async Task<Response<List<GetTimeLogDto>>> GetAllAsync()
    {
        var timeLogs = await context.TimeLogs
            .Include(x => x.User)
            .ToListAsync();

        var taskIds = timeLogs.Select(x => x.TaskId).Distinct().ToList();
        var tasks = await context.Tasks.Where(x => taskIds.Contains(x.Id)).ToListAsync();
        var teamIds = tasks.Select(x => x.TeamId).Distinct().ToList();
        var teams = await context.Teams.Where(x => teamIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var projectIds = teams.Values.Select(x => x.ProjectId).Distinct().ToList();
        var projects = await context.Projects.Where(x => projectIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var userIds = timeLogs.Select(x => x.UserId)
            .Concat(tasks.Where(x => !string.IsNullOrWhiteSpace(x.AssignedToId)).Select(x => x.AssignedToId))
            .Concat(tasks.Where(x => !string.IsNullOrWhiteSpace(x.CreatedById)).Select(x => x.CreatedById))
            .Concat(teams.Values.Where(x => !string.IsNullOrWhiteSpace(x.TeamLeadId)).Select(x => x.TeamLeadId!))
            .Concat(projects.Values.Select(x => x.EmployerId))
            .Distinct()
            .ToList();

        var users = await context.Users.Where(x => userIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var tasksById = tasks.ToDictionary(x => x.Id);

        var result = timeLogs
            .Where(x => tasksById.ContainsKey(x.TaskId))
            .Select(timeLog => new GetTimeLogDto
            {
                Id = timeLog.Id,
                TaskId = timeLog.TaskId,
                Task = ServiceMappingHelper.ToGetTaskDto(tasksById[timeLog.TaskId], teams, projects, users),
                UserId = timeLog.UserId,
                User = ServiceMappingHelper.ToGetUserDto(timeLog.User),
                StartedAt = timeLog.StartedAt,
                EndedAt = timeLog.EndedAt,
                DurationMinutes = timeLog.DurationMinutes,
                IsActive = timeLog.IsActive
            })
            .ToList();

        return new Response<List<GetTimeLogDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetTimeLogDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Date?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetTimeLogDto>(HttpStatusCode.NotFound, "TimeLog not found");
        }

        return new Response<GetTimeLogDto>(HttpStatusCode.OK, "ok", item);
    }
}
