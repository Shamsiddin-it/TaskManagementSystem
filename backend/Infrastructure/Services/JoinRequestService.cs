using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class JoinRequestService(ApplicationDbContext dbContext) : IJoinRequestService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateJoinRequestDto dto)
    {
        if (!await context.Teams.AnyAsync(x => x.Id == dto.TeamId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "Team not found");
        }

        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var joinRequest = new JoinRequest
        {
            TeamId = dto.TeamId,
            UserId = dto.UserId,
            Status = dto.Status,
            CoverMessage = dto.CoverMessage,
            CreatedAt = DateTime.UtcNow
        };

        await context.JoinRequests.AddAsync(joinRequest);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add JoinRequest successfully");
    }

    public Task<Response<string>> ApplyToTeamAsync(CreateJoinRequestDto dto) => AddAsync(dto);

    public async Task<Response<string>> UpdateAsync(Guid id, UpdateJoinRequestDto dto)
    {
        var joinRequest = await context.JoinRequests.FindAsync(id);
        if (joinRequest == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "JoinRequest not found");
        }

        if (dto.Status.HasValue) joinRequest.Status = dto.Status.Value;
        if (dto.CoverMessage != null) joinRequest.CoverMessage = dto.CoverMessage;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update JoinRequest successfully");
    }

    public async Task<Response<string>> DeleteAsync(Guid id)
    {
        var joinRequest = await context.JoinRequests.FindAsync(id);
        if (joinRequest == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "JoinRequest not found");
        }

        context.JoinRequests.Remove(joinRequest);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete JoinRequest successfully");
    }

    public async Task<Response<List<GetJoinRequestDto>>> GetAllAsync()
    {
        var joinRequests = await context.JoinRequests
            .Include(x => x.Team)
                .ThenInclude(x => x.Project)
                    .ThenInclude(x => x.Employer)
            .Include(x => x.User)
            .ToListAsync();

        var teamLeadIds = joinRequests
            .Where(x => !string.IsNullOrWhiteSpace(x.Team.TeamLeadId))
            .Select(x => x.Team.TeamLeadId!)
            .Distinct()
            .ToList();
        var teamLeads = await context.Users.Where(x => teamLeadIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

        var result = joinRequests.Select(joinRequest =>
        {
            GetUserDto? teamLead = null;
            if (!string.IsNullOrWhiteSpace(joinRequest.Team.TeamLeadId) && teamLeads.TryGetValue(joinRequest.Team.TeamLeadId, out var teamLeadUser))
            {
                teamLead = ServiceMappingHelper.ToGetUserDto(teamLeadUser);
            }

            var projectDto = ServiceMappingHelper.ToGetProjectDto(joinRequest.Team.Project);
            var teamDto = ServiceMappingHelper.ToGetTeamDto(joinRequest.Team, projectDto, teamLead);

            return new GetJoinRequestDto
            {
                Id = joinRequest.Id,
                TeamId = joinRequest.TeamId,
                Team = teamDto,
                UserId = joinRequest.UserId,
                User = ServiceMappingHelper.ToGetUserDto(joinRequest.User),
                Status = joinRequest.Status,
                CoverMessage = joinRequest.CoverMessage,
                CreatedAt = joinRequest.CreatedAt
            };
        }).ToList();

        return new Response<List<GetJoinRequestDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetJoinRequestDto>> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        var item = all.Date?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetJoinRequestDto>(HttpStatusCode.NotFound, "JoinRequest not found");
        }

        return new Response<GetJoinRequestDto>(HttpStatusCode.OK, "ok", item);
    }
}
