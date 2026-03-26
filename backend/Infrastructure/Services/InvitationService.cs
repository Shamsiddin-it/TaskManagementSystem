using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class InvitationService(ApplicationDbContext dbContext) : IInvitationService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateInvitationDto dto)
    {
        if (!await context.Teams.AnyAsync(x => x.Id == dto.TeamId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Team not found");
        }

        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        if (!await context.Users.AnyAsync(x => x.Id == dto.InvitedById))
        {
            return new Response<string>(HttpStatusCode.NotFound, "InvitedBy user not found");
        }

        var invitation = new Invitation
        {
            TeamId = dto.TeamId,
            UserId = dto.UserId,
            InvitedById = dto.InvitedById,
            Status = dto.Status,
            Message = dto.Message,
            ExpiresAt = dto.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        };

        await context.Invitations.AddAsync(invitation);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add Invitation successfully");
    }

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateInvitationDto dto)
    {
        var invitation = await context.Invitations.FindAsync(id);
        if (invitation == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Invitation not found");
        }

        if (dto.Status.HasValue) invitation.Status = dto.Status.Value;
        if (dto.Message != null) invitation.Message = dto.Message;
        if (dto.ExpiresAt.HasValue) invitation.ExpiresAt = dto.ExpiresAt.Value;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update Invitation successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var invitation = await context.Invitations.FindAsync(id);
        if (invitation == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Invitation not found");
        }

        context.Invitations.Remove(invitation);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete Invitation successfully");
    }

    public async Task<Response<string>> AcceptAsync(Guid id)
    {
        var invitation = await context.Invitations.FindAsync(id);
        if (invitation == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Invitation not found");
        }

        if (!await context.Teams.AnyAsync(x => x.Id == invitation.TeamId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Team not found");
        }

        if (!await context.Users.AnyAsync(x => x.Id == invitation.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        invitation.Status = InvitationStatus.Accepted;

        var exists = await context.TeamMembers.AnyAsync(x => x.TeamId == invitation.TeamId && x.UserId == invitation.UserId);
        if (!exists)
        {
            await context.TeamMembers.AddAsync(new TeamMember
            {
                TeamId = invitation.TeamId,
                UserId = invitation.UserId,
                DevRole = DevRole.Fullstack,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Invitation accepted successfully");
    }

    public async Task<Response<string>> RejectAsync(Guid id)
    {
        var invitation = await context.Invitations.FindAsync(id);
        if (invitation == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Invitation not found");
        }

        invitation.Status = InvitationStatus.Rejected;
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Invitation rejected successfully");
    }

    public async Task<Response<List<GetInvitationDto>>> GetAllAsync()
    {
        var invitations = await context.Invitations
            .Include(x => x.Team)
                .ThenInclude(x => x.Project)
                    .ThenInclude(x => x.Employer)
            .Include(x => x.User)
            .ToListAsync();

        var userIds = invitations
            .Where(x => !string.IsNullOrWhiteSpace(x.Team.TeamLeadId))
            .Select(x => x.Team.TeamLeadId!)
            .Concat(invitations.Select(x => x.InvitedById))
            .Distinct()
            .ToList();

        var users = await context.Users.Where(x => userIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var result = invitations.Select(invitation =>
        {
            users.TryGetValue(invitation.InvitedById, out var invitedBy);
            GetUserDto? teamLead = null;
            if (!string.IsNullOrWhiteSpace(invitation.Team.TeamLeadId) && users.TryGetValue(invitation.Team.TeamLeadId, out var teamLeadUser))
            {
                teamLead = ServiceMappingHelper.ToGetUserDto(teamLeadUser);
            }

            var projectDto = ServiceMappingHelper.ToGetProjectDto(invitation.Team.Project);
            var teamDto = ServiceMappingHelper.ToGetTeamDto(invitation.Team, projectDto, teamLead);

            return new GetInvitationDto
            {
                Id = invitation.Id,
                TeamId = invitation.TeamId,
                Team = teamDto,
                UserId = invitation.UserId,
                User = ServiceMappingHelper.ToGetUserDto(invitation.User),
                InvitedById = invitation.InvitedById,
                InvitedBy = invitedBy != null ? ServiceMappingHelper.ToGetUserDto(invitedBy) : ServiceMappingHelper.ToGetUserDto(invitation.User),
                Status = invitation.Status,
                Message = invitation.Message,
                ExpiresAt = invitation.ExpiresAt,
                CreatedAt = invitation.CreatedAt
            };
        }).ToList();

        return new Response<List<GetInvitationDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetInvitationDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Data?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetInvitationDto>(HttpStatusCode.NotFound, "Invitation not found");
        }

        return new Response<GetInvitationDto>(HttpStatusCode.OK, "ok", item);
    }
}
