using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AttachmentService(ApplicationDbContext dbContext) : IAttachmentService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateAttachmentDto dto)
    {
        if (!await context.Tasks.AnyAsync(x => x.Id == dto.TaskId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Task not found");
        }

        if (!await context.Users.AnyAsync(x => x.Id == dto.UploadedById))
        {
            return new Response<string>(HttpStatusCode.NotFound, "UploadedBy user not found");
        }

        var attachment = new Attachment
        {
            TaskId = dto.TaskId,
            UploadedById = dto.UploadedById,
            FileName = dto.FileName,
            FilePath = dto.FilePath,
            FileSize = dto.FileSize,
            FileType = dto.FileType,
            CreatedAt = DateTime.UtcNow
        };

        await context.Attachments.AddAsync(attachment);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add Attachment successfully");
    }

    public async Task<Response<string>> UpdateAsync(int id, UpdateAttachmentDto dto)
    {
        var attachment = await context.Attachments.FindAsync(id);
        if (attachment == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Attachment not found");
        }

        if (dto.FileName != null) attachment.FileName = dto.FileName;
        if (dto.FilePath != null) attachment.FilePath = dto.FilePath;
        if (dto.FileSize.HasValue) attachment.FileSize = dto.FileSize.Value;
        if (dto.FileType != null) attachment.FileType = dto.FileType;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update Attachment successfully");
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var attachment = await context.Attachments.FindAsync(id);
        if (attachment == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Attachment not found");
        }

        context.Attachments.Remove(attachment);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete Attachment successfully");
    }

    public async Task<Response<List<GetAttachmentDto>>> GetAllAsync()
    {
        var attachments = await context.Attachments
            .Include(x => x.UploadedBy)
            .ToListAsync();

        var taskIds = attachments.Select(x => x.TaskId).Distinct().ToList();
        var tasks = await context.Tasks.Where(x => taskIds.Contains(x.Id)).ToListAsync();
        var teamIds = tasks.Select(x => x.TeamId).Distinct().ToList();
        var teams = await context.Teams.Where(x => teamIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var projectIds = teams.Values.Select(x => x.ProjectId).Distinct().ToList();
        var projects = await context.Projects.Where(x => projectIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var userIds = attachments.Select(x => x.UploadedById)
            .Concat(tasks.Where(x => x.AssignedTo.HasValue).Select(x => x.AssignedTo!.Value))
            .Concat(tasks.Where(x => x.CreatedBy.HasValue).Select(x => x.CreatedBy!.Value))
            .Concat(teams.Values.Where(x => x.TeamLeadId.HasValue).Select(x => x.TeamLeadId!.Value))
            .Concat(projects.Values.Select(x => x.EmployerId))
            .Distinct()
            .ToList();

        var users = await context.Users.Where(x => userIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var tasksById = tasks.ToDictionary(x => x.Id);

        var result = attachments
            .Where(x => tasksById.ContainsKey(x.TaskId))
            .Select(attachment => new GetAttachmentDto
            {
                Id = attachment.Id,
                TaskId = attachment.TaskId,
                Task = ServiceMappingHelper.ToGetTaskDto(tasksById[attachment.TaskId], teams, projects, users),
                UploadedById = attachment.UploadedById,
                UploadedBy = ServiceMappingHelper.ToGetUserDto(attachment.UploadedBy),
                FileName = attachment.FileName,
                FilePath = attachment.FilePath,
                FileSize = attachment.FileSize,
                FileType = attachment.FileType,
                CreatedAt = attachment.CreatedAt
            })
            .ToList();

        return new Response<List<GetAttachmentDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetAttachmentDto>> GetByIdAsync(int id)
    {
        var all = await GetAllAsync();
        var item = all.Date?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetAttachmentDto>(HttpStatusCode.NotFound, "Attachment not found");
        }

        return new Response<GetAttachmentDto>(HttpStatusCode.OK, "ok", item);
    }
}
