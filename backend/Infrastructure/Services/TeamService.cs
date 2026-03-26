using Microsoft.EntityFrameworkCore;
using Domain.Models;
using System.Net;

public class TeamService : ITeamService
{
    private readonly ApplicationDbContext _db;
    public TeamService(ApplicationDbContext db) => _db = db;

    public async Task<Response<TeamResponseDto>> CreateTeamAsync(Guid projectId, CreateTeamDto dto)
    {
        try
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null)
                return new Response<TeamResponseDto>(
                    HttpStatusCode.NotFound, "Project not found");

            var team = new Team
            {
                ProjectId = projectId,
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            return new Response<TeamResponseDto>(
                HttpStatusCode.Created, "Team created successfully", MapToDto(team, 0, 0));
        }
        catch (Exception ex)
        {
            return new Response<TeamResponseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<TeamResponseDto>> UpdateTeamAsync(Guid teamId, UpdateTeamDto dto)
    {
        try
        {
            var team = await _db.Teams
                .Include(t => t.TeamLead)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return new Response<TeamResponseDto>(
                    HttpStatusCode.NotFound, "Team not found");

            if (dto.Name != null) team.Name = dto.Name;
            if (dto.Description != null) team.Description = dto.Description;
            team.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var memberCount = await _db.TeamMembers
                .CountAsync(m => m.TeamId == teamId && m.IsActive);

            return new Response<TeamResponseDto>(
                HttpStatusCode.OK, "Team updated successfully",
                MapToDto(team, memberCount, 0));
        }
        catch (Exception ex)
        {
            return new Response<TeamResponseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> AssignTeamLeadAsync(Guid teamId, string teamLeadId)
    {
        try
        {
            var team = await _db.Teams.FindAsync(teamId);
            if (team == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Team not found");

            var user = await _db.Users.FindAsync(teamLeadId);
            if (user == null)
                return new Response<bool>(HttpStatusCode.NotFound, "User not found");

            user.Role = UserRole.TeamLead;
            team.TeamLeadId = teamLeadId;
            team.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Team lead assigned successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> RemoveTeamLeadAsync(Guid teamId)
    {
        try
        {
            var team = await _db.Teams.FindAsync(teamId);
            if (team == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Team not found");

            if (!string.IsNullOrEmpty(team.TeamLeadId))
            {
                var prevLead = await _db.Users.FindAsync(team.TeamLeadId);
                if (prevLead != null)
                    prevLead.Role = UserRole.Worker;
            }

            team.TeamLeadId = null;
            team.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Team lead removed successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<TeamResponseDto>> GetTeamProgressAsync(Guid teamId)
    {
        try
        {
            var team = await _db.Teams
                .Include(t => t.TeamLead)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return new Response<TeamResponseDto>(
                    HttpStatusCode.NotFound, "Team not found");

            var tasks = await _db.Tasks
                .Where(t => t.TeamId == teamId)
                .ToListAsync();

            int completionPercent = tasks.Count == 0 ? 0 :
                (int)((double)tasks.Count(t => t.Status == TaskStatus.Done)
                      / tasks.Count * 100);

            int memberCount = await _db.TeamMembers
                .CountAsync(m => m.TeamId == teamId && m.IsActive);

            return new Response<TeamResponseDto>(
                HttpStatusCode.OK, "Team progress retrieved successfully",
                MapToDto(team, memberCount, completionPercent));
        }
        catch (Exception ex)
        {
            return new Response<TeamResponseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private static TeamResponseDto MapToDto(Team t, int memberCount, int completionPercent) =>
        new()
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            ProjectId = t.ProjectId,
            TeamLead = t.TeamLead == null ? null : new TeamLeadDto
            {
                Id = t.TeamLead.Id,
                FullName = t.TeamLead.FirstName + " " + t.TeamLead.LastName,
                AvatarInitials = t.TeamLead.AvatarInitials
            },
            MemberCount = memberCount,
            CompletionPercent = completionPercent
        };
}
