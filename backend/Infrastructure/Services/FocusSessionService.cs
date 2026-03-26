using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class FocusSessionService(ApplicationDbContext dbContext) : IFocusSessionService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateFocusSessionDto dto)
    {
        return await CreateSessionAsync(dto);
    }

    public async Task<Response<string>> StartAsync(CreateFocusSessionDto dto)
    {
        dto.Status = FocusSessionStatus.Active;
        if (dto.StartedAt == default)
        {
            dto.StartedAt = DateTime.UtcNow;
        }

        return await CreateSessionAsync(dto);
    }

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateFocusSessionDto dto)
    {
        var session = await context.FocusSessions.FindAsync(id);
        if (session == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "FocusSession not found");
        }

        if (dto.TaskId.HasValue)
        {
            if (!await context.Tasks.AnyAsync(x => x.Id == dto.TaskId.Value))
            {
                return new Response<string>(HttpStatusCode.NotFound, "Task not found");
            }

            session.TaskId = dto.TaskId.Value;
        }

        if (dto.StartedAt.HasValue) session.StartedAt = dto.StartedAt.Value;
        if (dto.EndedAt.HasValue) session.EndedAt = dto.EndedAt.Value;
        if (dto.DurationMinutes.HasValue) session.DurationMinutes = dto.DurationMinutes.Value;
        if (dto.Status.HasValue) session.Status = dto.Status.Value;
        if (dto.Notes != null) session.Notes = dto.Notes;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update FocusSession successfully");
    }

    public async Task<Response<string>> PauseAsync(Guid id)
    {
        var session = await context.FocusSessions.FindAsync(id);
        if (session == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "FocusSession not found");
        }

        session.Status = FocusSessionStatus.Paused;
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "FocusSession paused successfully");
    }

    public async Task<Response<string>> CompleteAsync(Guid id)
    {
        var session = await context.FocusSessions.FindAsync(id);
        if (session == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "FocusSession not found");
        }

        session.Status = FocusSessionStatus.Completed;
        session.EndedAt ??= DateTime.UtcNow;
        if (!session.DurationMinutes.HasValue)
        {
            session.DurationMinutes = (int)Math.Max(0, Math.Round((session.EndedAt.Value - session.StartedAt).TotalMinutes));
        }

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "FocusSession completed successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var session = await context.FocusSessions.FindAsync(id);
        if (session == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "FocusSession not found");
        }

        context.FocusSessions.Remove(session);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete FocusSession successfully");
    }

    public async Task<Response<List<GetFocusSessionDto>>> GetAllAsync()
    {
        var sessions = await context.FocusSessions
            .Include(x => x.User)
            .ToListAsync();

        var taskIds = sessions.Where(x => x.TaskId.HasValue).Select(x => x.TaskId!.Value).Distinct().ToList();
        var tasks = await context.Tasks.Where(x => taskIds.Contains(x.Id)).ToListAsync();
        var teamIds = tasks.Select(x => x.TeamId).Distinct().ToList();
        var teams = await context.Teams.Where(x => teamIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var projectIds = teams.Values.Select(x => x.ProjectId).Distinct().ToList();
        var projects = await context.Projects.Where(x => projectIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var userIds = sessions.Select(x => x.UserId)
            .Concat(tasks.Where(x => !string.IsNullOrWhiteSpace(x.AssignedToId)).Select(x => x.AssignedToId))
            .Concat(tasks.Where(x => !string.IsNullOrWhiteSpace(x.CreatedById)).Select(x => x.CreatedById))
            .Concat(teams.Values.Where(x => !string.IsNullOrWhiteSpace(x.TeamLeadId)).Select(x => x.TeamLeadId!))
            .Concat(projects.Values.Select(x => x.EmployerId))
            .Distinct()
            .ToList();

        var users = await context.Users.Where(x => userIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var tasksById = tasks.ToDictionary(x => x.Id);

        var result = sessions.Select(session => new GetFocusSessionDto
        {
            Id = session.Id,
            UserId = session.UserId,
            User = ServiceMappingHelper.ToGetUserDto(session.User),
            TaskId = session.TaskId,
            Task = session.TaskId.HasValue && tasksById.ContainsKey(session.TaskId.Value)
                ? ServiceMappingHelper.ToGetTaskDto(tasksById[session.TaskId.Value], teams, projects, users)
                : null,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt,
            DurationMinutes = session.DurationMinutes,
            Status = session.Status,
            Notes = session.Notes,
            CreatedAt = session.CreatedAt
        }).ToList();

        return new Response<List<GetFocusSessionDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetFocusSessionDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Date?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetFocusSessionDto>(HttpStatusCode.NotFound, "FocusSession not found");
        }

        return new Response<GetFocusSessionDto>(HttpStatusCode.OK, "ok", item);
    }

    private async Task<Response<string>> CreateSessionAsync(CreateFocusSessionDto dto)
    {
        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        if (dto.TaskId.HasValue && !await context.Tasks.AnyAsync(x => x.Id == dto.TaskId.Value))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Task not found");
        }

        var session = new FocusSession
        {
            UserId = dto.UserId,
            TaskId = dto.TaskId,
            StartedAt = dto.StartedAt == default ? DateTime.UtcNow : dto.StartedAt,
            EndedAt = dto.EndedAt,
            DurationMinutes = dto.DurationMinutes,
            Status = dto.Status,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await context.FocusSessions.AddAsync(session);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add FocusSession successfully");
    }
}
