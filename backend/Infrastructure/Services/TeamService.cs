using System.Net;
using System.Security.Claims;
using Task = System.Threading.Tasks.Task;
using Application.DTOs;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TeamService : ITeamService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeamService(
        ApplicationDbContext db,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<Response<TeamResponseDto>> CreateTeamAsync(Guid projectId, CreateTeamDto dto)
    {
        try
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null)
            {
                return new Response<TeamResponseDto>(HttpStatusCode.NotFound, "Project not found");
            }

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

            team = await _db.Teams
                .Include(t => t.TeamLead)
                .FirstAsync(t => t.Id == team.Id);

            return new Response<TeamResponseDto>(
                HttpStatusCode.Created,
                "Team created successfully",
                MapToDto(team, 0, 0));
        }
        catch (Exception ex)
        {
            return new Response<TeamResponseDto>(HttpStatusCode.InternalServerError, ex.Message);
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
            {
                return new Response<TeamResponseDto>(HttpStatusCode.NotFound, "Team not found");
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                team.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                team.Description = dto.Description;
            }

            team.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var memberCount = await _db.TeamMembers
                .CountAsync(m => m.TeamId == teamId && m.IsActive);

            team = await _db.Teams
                .Include(t => t.TeamLead)
                .FirstAsync(t => t.Id == teamId);

            return new Response<TeamResponseDto>(
                HttpStatusCode.OK,
                "Team updated successfully",
                MapToDto(team, memberCount, 0));
        }
        catch (Exception ex)
        {
            return new Response<TeamResponseDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> AssignTeamLeadAsync(Guid teamId, string teamLeadId)
    {
        try
        {
            var team = await _db.Teams.FindAsync(teamId);
            if (team == null)
            {
                return new Response<bool>(HttpStatusCode.NotFound, "Team not found");
            }

            var previousLeadId = team.TeamLeadId;

            if (string.IsNullOrWhiteSpace(teamLeadId))
            {
                return new Response<bool>(HttpStatusCode.BadRequest, "Team lead id is required");
            }

            var user = await _db.Users.FindAsync(teamLeadId);
            if (user == null)
            {
                return new Response<bool>(HttpStatusCode.NotFound, "User not found");
            }

            team.TeamLeadId = teamLeadId;
            team.UpdatedAt = DateTime.UtcNow;

            await PromoteUserToTeamLeadAsync(teamLeadId);

            if (!string.IsNullOrWhiteSpace(previousLeadId) && previousLeadId != teamLeadId)
            {
                await DemoteUserIfNoLongerLeadingAsync(previousLeadId);
            }

            await _db.SaveChangesAsync();

            return new Response<bool>(HttpStatusCode.OK, "Team lead assigned successfully", true);
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
            {
                return new Response<bool>(HttpStatusCode.NotFound, "Team not found");
            }

            var previousLeadId = team.TeamLeadId;

            team.TeamLeadId = null;
            team.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(previousLeadId))
            {
                await DemoteUserIfNoLongerLeadingAsync(previousLeadId);
            }

            await _db.SaveChangesAsync();

            return new Response<bool>(HttpStatusCode.OK, "Team lead removed successfully", true);
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
            {
                return new Response<TeamResponseDto>(HttpStatusCode.NotFound, "Team not found");
            }

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = user?.FindFirstValue(ClaimTypes.Role);

            if (role == "Team Lead" && team.TeamLeadId != userId)
            {
                return new Response<TeamResponseDto>(HttpStatusCode.Forbidden, "You do not have access to this team.");
            }

            var tasks = await _db.Tasks
                .Where(t => t.TeamId == teamId)
                .ToListAsync();

            var completionPercent = tasks.Count == 0
                ? 0
                : (int)((double)tasks.Count(t => t.Status == TaskStatus.Done) / tasks.Count * 100);

            var memberCount = await _db.TeamMembers
                .CountAsync(m => m.TeamId == teamId && m.IsActive);

            return new Response<TeamResponseDto>(
                HttpStatusCode.OK,
                "Team progress retrieved successfully",
                MapToDto(team, memberCount, completionPercent));
        }
        catch (Exception ex)
        {
            return new Response<TeamResponseDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private async Task PromoteUserToTeamLeadAsync(string userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null)
        {
            user.Role = UserRole.TeamLead;
            user.UpdatedAt = DateTime.UtcNow;
            await SyncIdentityRoleAsync(user, "Team Lead");
        }
    }

    private async Task DemoteUserIfNoLongerLeadingAsync(string userId)
    {
        var stillLeadsAnotherTeam = await _db.Teams
            .AnyAsync(t => t.TeamLeadId == userId);

        if (stillLeadsAnotherTeam)
        {
            return;
        }

        var user = await _db.Users.FindAsync(userId);
        if (user != null && user.Role == UserRole.TeamLead)
        {
            user.Role = UserRole.Worker;
            user.UpdatedAt = DateTime.UtcNow;
            await SyncIdentityRoleAsync(user, "Worker");
        }
    }

    private async Task SyncIdentityRoleAsync(ApplicationUser user, string expectedRole)
    {
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(expectedRole))
        {
            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, expectedRole);
        }
    }

    private static TeamResponseDto MapToDto(Team team, int memberCount, int completionPercent) =>
        new()
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            ProjectId = team.ProjectId,
            TeamLead = team.TeamLead == null
                ? null
                : new TeamLeadDto
                {
                    Id = team.TeamLead.Id,
                    FullName = team.TeamLead.FullName,
                    AvatarInitials = team.TeamLead.AvatarInitials
                },
            MemberCount = memberCount,
            CompletionPercent = completionPercent
        };
}
