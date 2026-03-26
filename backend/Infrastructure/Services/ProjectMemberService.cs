using System.Net;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

public class ProjectMemberService : IProjectMemberService
{
    private readonly ApplicationDbContext _db;
    public ProjectMemberService(ApplicationDbContext db) => _db = db;

    public async Task<Response<ProjectMemberDto>> AddMemberAsync(Guid projectId, AddProjectMemberDto dto)
    {
        try
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                return new Response<ProjectMemberDto>(HttpStatusCode.NotFound, "Project not found");

            var already = await _db.ProjectMembers
                .AnyAsync(m => m.ProjectId == projectId && m.UserId == dto.UserId);
            if (already)
                return new Response<ProjectMemberDto>(
                    HttpStatusCode.Conflict, "User already in this project");

            var user = await _db.Users.FindAsync(dto.UserId);
            if (user == null)
                return new Response<ProjectMemberDto>(
                    HttpStatusCode.NotFound, "User not found");

            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = dto.UserId,
                ProjectRole = dto.ProjectRole,
                Availability = MemberAvailability.Available,
                JoinedAt = DateTime.UtcNow
            };

            _db.ProjectMembers.Add(member);
            var project = await _db.Projects.FindAsync(projectId);
            if (project != null)
            {
                _db.EmployerNotifications.Add(new EmployerNotification
                {
                    EmployerId = project.EmployerId,
                    Type = EmployerNotifType.TeamUpdate,
                    Priority = NotifPriority.Normal,
                    Title = $"{user.FirstName} added to {project.Title}",
                    Body = $"{dto.ProjectRole} assigned to the project.",
                    ActionLabel = "View Detail",
                    RelatedProjectId = projectId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _db.SaveChangesAsync();

            var result = new ProjectMemberDto
            {
                UserId = member.UserId,
                FullName = user.FirstName + " " + user.LastName,
                AvatarInitials = user.AvatarInitials,
                AvatarColor = user.AvatarColor,
                ProjectRole = member.ProjectRole,
                Availability = member.Availability
            };

            return new Response<ProjectMemberDto>(
                HttpStatusCode.Created, "Member added successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<ProjectMemberDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> RemoveMemberAsync(Guid projectId, string userId)
    {
        try
        {
            var member = await _db.ProjectMembers
                .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);
            if (member == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Member not found");

            _db.ProjectMembers.Remove(member);
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Member removed successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<List<ProjectMemberDto>>> GetMembersAsync(Guid projectId)
    {
        try
        {
            var result = await _db.ProjectMembers
                .Where(m => m.ProjectId == projectId)
                .ToListAsync();

            var userIds = result.Select(x => x.UserId).Distinct().ToList();
            var users = await _db.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var mapped = result.Select(m =>
            {
                users.TryGetValue(m.UserId, out var user);

                return new ProjectMemberDto
                {
                    UserId = m.UserId,
                    FullName = user?.FirstName + " " + user?.LastName ?? string.Empty,
                    AvatarInitials = user?.AvatarInitials,
                    AvatarColor = user?.AvatarColor,
                    ProjectRole = m.ProjectRole,
                    Availability = m.Availability
                };
            }).ToList();

            return new Response<List<ProjectMemberDto>>(
                HttpStatusCode.OK, "Members retrieved successfully", mapped);
        }
        catch (Exception ex)
        {
            return new Response<List<ProjectMemberDto>>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> UpdateMemberRoleAsync(Guid projectId, string userId, string role)
    {
        try
        {
            var member = await _db.ProjectMembers
                .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);
            if (member == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Member not found");

            member.ProjectRole = role;
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Role updated successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
