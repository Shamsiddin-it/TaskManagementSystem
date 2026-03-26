using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TaskCommentService(ApplicationDbContext dbContext) : ITaskCommentService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateTaskCommentDto dto)
    {
        if (!await context.Tasks.AnyAsync(x => x.Id == dto.TaskId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Task not found");
        }

        if (!await context.Users.AnyAsync(x => x.Id == dto.AuthorId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Author not found");
        }

        var taskComment = new TaskComment
        {
            TaskId = dto.TaskId,
            AuthorId = dto.AuthorId,
            Message = dto.Message,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };

        await context.TaskComments.AddAsync(taskComment);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add TaskComment successfully");
    }

    public Task<Response<string>> AddCommentAsync(CreateTaskCommentDto dto) => AddAsync(dto);

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateTaskCommentDto dto)
    {
        var taskComment = await context.TaskComments.FindAsync(id);
        if (taskComment == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "TaskComment not found");
        }

        if (dto.Message != null) taskComment.Message = dto.Message;
        if (dto.Type.HasValue) taskComment.Type = dto.Type.Value;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update TaskComment successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var taskComment = await context.TaskComments.FindAsync(id);
        if (taskComment == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "TaskComment not found");
        }

        context.TaskComments.Remove(taskComment);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete TaskComment successfully");
    }

    public async Task<Response<List<GetTaskCommentDto>>> GetAllAsync()
    {
        var comments = await context.TaskComments.Include(x => x.Author).ToListAsync();
        var taskIds = comments.Select(x => x.TaskId).Distinct().ToList();
        var tasks = await context.Tasks.Where(x => taskIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var teamIds = tasks.Values.Select(x => x.TeamId).Distinct().ToList();
        var teams = await context.Teams.Include(x => x.Project).ThenInclude(x => x.Employer).Where(x => teamIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var userIds = comments.Select(x => x.AuthorId)
            .Concat(tasks.Values.Where(x => !string.IsNullOrWhiteSpace(x.AssignedToId)).Select(x => x.AssignedToId))
            .Concat(tasks.Values.Where(x => !string.IsNullOrWhiteSpace(x.CreatedById)).Select(x => x.CreatedById))
            .Concat(teams.Values.Where(x => !string.IsNullOrWhiteSpace(x.TeamLeadId)).Select(x => x.TeamLeadId!))
            .Distinct()
            .ToList();

        var users = await context.Users.Where(x => userIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var result = comments.Where(x => tasks.ContainsKey(x.TaskId)).Select(comment =>
        {
            var task = tasks[comment.TaskId];
            var team = teams[task.TeamId];
            users.TryGetValue(task.AssignedToId ?? string.Empty, out var assignedUser);
            users.TryGetValue(task.CreatedById ?? string.Empty, out var createdByUser);
            users.TryGetValue(team.TeamLeadId ?? string.Empty, out var teamLeadUser);

            var projectDto = ServiceMappingHelper.ToGetProjectDto(team.Project);
            var teamDto = ServiceMappingHelper.ToGetTeamDto(team, projectDto, teamLeadUser != null ? ServiceMappingHelper.ToGetUserDto(teamLeadUser) : null);
            var taskDto = ServiceMappingHelper.ToGetTaskDto(task, teamDto,
                assignedUser != null ? ServiceMappingHelper.ToGetUserDto(assignedUser) : null,
                createdByUser != null ? ServiceMappingHelper.ToGetUserDto(createdByUser) : null);

            return new GetTaskCommentDto
            {
                Id = comment.Id,
                TaskId = comment.TaskId,
                Task = taskDto,
                AuthorId = comment.AuthorId,
                Author = ServiceMappingHelper.ToGetUserDto(comment.Author),
                Message = comment.Message,
                Type = comment.Type,
                CreatedAt = comment.CreatedAt
            };
        }).ToList();

        return new Response<List<GetTaskCommentDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetTaskCommentDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Data?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetTaskCommentDto>(HttpStatusCode.NotFound, "TaskComment not found");
        }

        return new Response<GetTaskCommentDto>(HttpStatusCode.OK, "ok", item);
    }
}
