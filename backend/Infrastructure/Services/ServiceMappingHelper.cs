using Application.DTOs;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services;

internal static class ServiceMappingHelper
{
    public static GetUserDto ToGetUserDto(User user)
    {
        Enum.TryParse<UserRole>(user.Role, true, out var role);

        return new GetUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = role,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static GetProjectDto ToGetProjectDto(Project project)
    {
        return new GetProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            EmployerId = project.EmployerId,
            Employer = ToGetUserDto(project.Employer),
            Status = project.Status,
            GlobalDeadline = project.GlobalDeadline,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }

    public static GetProjectDto ToGetProjectDto(Project project, GetUserDto employerDto)
    {
        return new GetProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            EmployerId = project.EmployerId,
            Employer = employerDto,
            Status = project.Status,
            GlobalDeadline = project.GlobalDeadline,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }

    public static GetTeamDto ToGetTeamDto(Team team, GetProjectDto projectDto, GetUserDto? teamLeadDto)
    {
        return new GetTeamDto
        {
            Id = team.Id,
            ProjectId = team.ProjectId,
            Project = projectDto,
            Name = team.Name,
            TeamLeadId = team.TeamLeadId,
            TeamLead = teamLeadDto,
            Description = team.Description,
            CreatedAt = team.CreatedAt
        };
    }

    public static GetTaskDto ToGetTaskDto(Domain.Entities.Task task, GetTeamDto teamDto, GetUserDto? assignedToDto, GetUserDto? createdByDto)
    {
        return new GetTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            TeamId = task.TeamId,
            Team = teamDto,
            AssignedTo = task.AssignedTo,
            AssignedToUser = assignedToDto,
            CreatedBy = task.CreatedBy,
            CreatedByUser = createdByDto,
            Status = task.Status,
            Priority = task.Priority,
            Deadline = task.Deadline,
            ScheduledStart = task.ScheduledStart,
            ScheduledEnd = task.ScheduledEnd,
            EstimatedHours = task.EstimatedHours,
            OrderIndex = task.OrderIndex,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    public static GetTeamDto ToGetTeamDto(Team team, Dictionary<int, Project> projects, Dictionary<int, User> users)
    {
        var project = projects[team.ProjectId];
        var employerDto = users.TryGetValue(project.EmployerId, out var employerUser)
            ? ToGetUserDto(employerUser)
            : new GetUserDto { Id = project.EmployerId };

        GetUserDto? teamLeadDto = null;
        if (team.TeamLeadId.HasValue && users.TryGetValue(team.TeamLeadId.Value, out var teamLeadUser))
        {
            teamLeadDto = ToGetUserDto(teamLeadUser);
        }

        return ToGetTeamDto(team, ToGetProjectDto(project, employerDto), teamLeadDto);
    }

    public static GetTaskDto ToGetTaskDto(Domain.Entities.Task task, Dictionary<int, Team> teams, Dictionary<int, Project> projects, Dictionary<int, User> users)
    {
        var team = teams[task.TeamId];
        var teamDto = ToGetTeamDto(team, projects, users);

        GetUserDto? assignedToDto = null;
        if (task.AssignedTo.HasValue && users.TryGetValue(task.AssignedTo.Value, out var assignedUser))
        {
            assignedToDto = ToGetUserDto(assignedUser);
        }

        GetUserDto? createdByDto = null;
        if (task.CreatedBy.HasValue && users.TryGetValue(task.CreatedBy.Value, out var createdByUser))
        {
            createdByDto = ToGetUserDto(createdByUser);
        }

        return ToGetTaskDto(task, teamDto, assignedToDto, createdByDto);
    }

    public static GetBadgeDto ToGetBadgeDto(Badge badge)
    {
        return new GetBadgeDto
        {
            Id = badge.Id,
            Name = badge.Name,
            Description = badge.Description,
            Condition = badge.Condition,
            IconUrl = badge.IconUrl,
            CreatedAt = badge.CreatedAt
        };
    }
}
