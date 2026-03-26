using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ScheduleEventService(ApplicationDbContext dbContext) : IScheduleEventService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateScheduleEventDto dto)
    {
        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        if (dto.TaskId.HasValue && !await context.Tasks.AnyAsync(x => x.Id == dto.TaskId.Value))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Task not found");
        }

        var scheduleEvent = new ScheduleEvent
        {
            UserId = dto.UserId,
            TaskId = dto.TaskId,
            Title = dto.Title,
            Type = dto.Type,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            ColorHex = dto.ColorHex,
            IsUrgent = dto.IsUrgent,
            CreatedAt = DateTime.UtcNow
        };

        await context.ScheduleEvents.AddAsync(scheduleEvent);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add ScheduleEvent successfully");
    }

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateScheduleEventDto dto)
    {
        var scheduleEvent = await context.ScheduleEvents.FindAsync(id);
        if (scheduleEvent == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "ScheduleEvent not found");
        }

        if (dto.TaskId.HasValue)
        {
            if (!await context.Tasks.AnyAsync(x => x.Id == dto.TaskId.Value))
            {
                return new Response<string>(HttpStatusCode.NotFound, "Task not found");
            }

            scheduleEvent.TaskId = dto.TaskId;
        }
        if (dto.Title != null) scheduleEvent.Title = dto.Title;
        if (dto.Type.HasValue) scheduleEvent.Type = dto.Type.Value;
        if (dto.StartTime.HasValue) scheduleEvent.StartTime = dto.StartTime.Value;
        if (dto.EndTime.HasValue) scheduleEvent.EndTime = dto.EndTime.Value;
        if (dto.ColorHex != null) scheduleEvent.ColorHex = dto.ColorHex;
        if (dto.IsUrgent.HasValue) scheduleEvent.IsUrgent = dto.IsUrgent.Value;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update ScheduleEvent successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var scheduleEvent = await context.ScheduleEvents.FindAsync(id);
        if (scheduleEvent == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "ScheduleEvent not found");
        }

        context.ScheduleEvents.Remove(scheduleEvent);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete ScheduleEvent successfully");
    }

    public async Task<Response<List<GetScheduleEventDto>>> GetAllAsync()
    {
        var events = await context.ScheduleEvents.Include(x => x.User).ToListAsync();
        var taskIds = events.Where(x => x.TaskId.HasValue).Select(x => x.TaskId!.Value).Distinct().ToList();
        var tasks = await context.Tasks.Where(x => taskIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var teamIds = tasks.Values.Select(x => x.TeamId).Distinct().ToList();
        var teams = await context.Teams.Include(x => x.Project).ThenInclude(x => x.Employer).Where(x => teamIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var userIds = events.Select(x => x.UserId)
            .Concat(tasks.Values.Where(x => !string.IsNullOrWhiteSpace(x.AssignedToId)).Select(x => x.AssignedToId))
            .Concat(tasks.Values.Where(x => !string.IsNullOrWhiteSpace(x.CreatedById)).Select(x => x.CreatedById))
            .Concat(teams.Values.Where(x => !string.IsNullOrWhiteSpace(x.TeamLeadId)).Select(x => x.TeamLeadId!))
            .Distinct()
            .ToList();

        var users = await context.Users.Where(x => userIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var result = events.Select(item =>
        {
            GetTaskDto? taskDto = null;
            if (item.TaskId.HasValue && tasks.TryGetValue(item.TaskId.Value, out var task) && teams.TryGetValue(task.TeamId, out var team))
            {
                users.TryGetValue(task.AssignedToId ?? string.Empty, out var assignedUser);
                users.TryGetValue(task.CreatedById ?? string.Empty, out var createdByUser);
                users.TryGetValue(team.TeamLeadId ?? string.Empty, out var teamLeadUser);

                var projectDto = ServiceMappingHelper.ToGetProjectDto(team.Project);
                var teamDto = ServiceMappingHelper.ToGetTeamDto(team, projectDto, teamLeadUser != null ? ServiceMappingHelper.ToGetUserDto(teamLeadUser) : null);
                taskDto = ServiceMappingHelper.ToGetTaskDto(task, teamDto,
                    assignedUser != null ? ServiceMappingHelper.ToGetUserDto(assignedUser) : null,
                    createdByUser != null ? ServiceMappingHelper.ToGetUserDto(createdByUser) : null);
            }

            return new GetScheduleEventDto
            {
                Id = item.Id,
                UserId = item.UserId,
                User = ServiceMappingHelper.ToGetUserDto(item.User),
                TaskId = item.TaskId,
                Task = taskDto,
                Title = item.Title,
                Type = item.Type,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                ColorHex = item.ColorHex,
                IsUrgent = item.IsUrgent,
                CreatedAt = item.CreatedAt
            };
        }).ToList();

        return new Response<List<GetScheduleEventDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetScheduleEventDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Data?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetScheduleEventDto>(HttpStatusCode.NotFound, "ScheduleEvent not found");
        }

        return new Response<GetScheduleEventDto>(HttpStatusCode.OK, "ok", item);
    }
}
