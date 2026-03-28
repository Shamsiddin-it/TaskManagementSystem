using Application.DTOs;
using Domain.Models;
using Domain.Enums;

namespace Infrastructure.Services;

internal static class ServiceMappingHelper
{
    public static Application.DTOs.GetUserDto ToGetUserDto(ApplicationUser user)
    {
        return new Application.DTOs.GetUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static Application.DTOs.GetProjectDto ToGetProjectDto(Project project)
    {
        return new Application.DTOs.GetProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description ?? string.Empty,
            EmployerId = project.EmployerId,
            Employer = project.Employer is null
                ? new Application.DTOs.GetUserDto { Id = project.EmployerId }
                : ToGetUserDto(project.Employer),
            Status = project.Status.ToString(),
            GlobalDeadline = project.GlobalDeadline,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }

    public static Application.DTOs.GetProjectDto ToGetProjectDto(Project project, Application.DTOs.GetUserDto employerDto)
    {
        return new Application.DTOs.GetProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description ?? string.Empty,
            EmployerId = project.EmployerId,
            Employer = employerDto,
            Status = project.Status.ToString(),
            GlobalDeadline = project.GlobalDeadline,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }

    public static Application.DTOs.GetTeamDto ToGetTeamDto(Team team, Application.DTOs.GetProjectDto projectDto, Application.DTOs.GetUserDto? teamLeadDto)
    {
        return new Application.DTOs.GetTeamDto
        {
            Id = team.Id,
            ProjectId = team.ProjectId,
            Project = projectDto,
            Name = team.Name,
            TeamLeadId = team.TeamLeadId,
            TeamLead = teamLeadDto,
            Description = team.Description ?? string.Empty,
            CreatedAt = team.CreatedAt
        };
    }

    public static Application.DTOs.GetTaskDto ToGetTaskDto(Domain.Models.Task task, Application.DTOs.GetTeamDto teamDto, Application.DTOs.GetUserDto? assignedToDto, Application.DTOs.GetUserDto? createdByDto)
    {
        return new Application.DTOs.GetTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description ?? string.Empty,
            TeamId = task.TeamId,
            Team = teamDto,
            AssignedTo = task.AssignedToId,
            AssignedToUser = assignedToDto,
            CreatedBy = task.CreatedById,
            CreatedByUser = createdByDto,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            Deadline = task.Deadline,
            ScheduledStart = task.ScheduledStart,
            ScheduledEnd = task.ScheduledEnd,
            EstimatedHours = task.EstimatedHours,
            OrderIndex = task.OrderIndex,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    public static Application.DTOs.GetTeamDto ToGetTeamDto(Team team, Dictionary<Guid, Project> projects, Dictionary<string, ApplicationUser> users)
    {
        var project = projects[team.ProjectId];
        var employerDto = users.TryGetValue(project.EmployerId, out var employerUser)
            ? ToGetUserDto(employerUser)
            : new Application.DTOs.GetUserDto { Id = project.EmployerId };

        Application.DTOs.GetUserDto? teamLeadDto = null;
        if (!string.IsNullOrWhiteSpace(team.TeamLeadId) && users.TryGetValue(team.TeamLeadId, out var teamLeadUser))
        {
            teamLeadDto = ToGetUserDto(teamLeadUser);
        }

        return ToGetTeamDto(team, ToGetProjectDto(project, employerDto), teamLeadDto);
    }

    public static Application.DTOs.GetTaskDto ToGetTaskDto(Domain.Models.Task task, Dictionary<Guid, Team> teams, Dictionary<Guid, Project> projects, Dictionary<string, ApplicationUser> users)
    {
        var team = teams[task.TeamId];
        var teamDto = ToGetTeamDto(team, projects, users);

        Application.DTOs.GetUserDto? assignedToDto = null;
        if (!string.IsNullOrWhiteSpace(task.AssignedToId) && users.TryGetValue(task.AssignedToId, out var assignedUser))
        {
            assignedToDto = ToGetUserDto(assignedUser);
        }

        Application.DTOs.GetUserDto? createdByDto = null;
        if (!string.IsNullOrWhiteSpace(task.CreatedById) && users.TryGetValue(task.CreatedById, out var createdByUser))
        {
            createdByDto = ToGetUserDto(createdByUser);
        }

        return ToGetTaskDto(task, teamDto, assignedToDto, createdByDto);
    }

    public static Application.DTOs.GetBadgeDto ToGetBadgeDto(Badge badge)
    {
        return new Application.DTOs.GetBadgeDto
        {
            Id = badge.Id,
            Name = badge.Name,
            Description = badge.Description ?? string.Empty,
            Condition = badge.Condition ?? string.Empty,
            IconUrl = badge.IconUrl,
            CreatedAt = badge.CreatedAt
        };
    }
}
