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
}
