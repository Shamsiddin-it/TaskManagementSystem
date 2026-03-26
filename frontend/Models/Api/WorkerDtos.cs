using System;
using System.Text.Json.Serialization;

namespace TaskManager.Web.Models.Api;

public enum FocusSessionStatus
{
    Active = 0,
    Paused = 1,
    Completed = 2
}

// Matches backend shapes closely enough for System.Text.Json (extra fields are ignored).
public sealed class GetDaySummaryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime SummaryDate { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksTotal { get; set; }
    public decimal FocusHours { get; set; }
    public int ProductivityScore { get; set; }
    public string? ProductivityGrade { get; set; }
    public int StreakDays { get; set; }
    public string? TomorrowPriority1 { get; set; }
    public string? TomorrowPriority2 { get; set; }
    public string? TomorrowPriority3 { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class CreateDaySummaryDto
{
    public int UserId { get; set; }
    public DateTime SummaryDate { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksTotal { get; set; }
    public decimal FocusHours { get; set; }
    public int ProductivityScore { get; set; }
    public string? ProductivityGrade { get; set; }
    public int StreakDays { get; set; }
    public string? TomorrowPriority1 { get; set; }
    public string? TomorrowPriority2 { get; set; }
    public string? TomorrowPriority3 { get; set; }
}

public sealed class UpdateDaySummaryDto
{
    public DateTime? SummaryDate { get; set; }
    public int? TasksCompleted { get; set; }
    public int? TasksTotal { get; set; }
    public decimal? FocusHours { get; set; }
    public int? ProductivityScore { get; set; }
    public string? ProductivityGrade { get; set; }
    public int? StreakDays { get; set; }
    public string? TomorrowPriority1 { get; set; }
    public string? TomorrowPriority2 { get; set; }
    public string? TomorrowPriority3 { get; set; }
}

public sealed class GetUserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    // Backend uses Domain.Enums.UserRole; in JSON it is usually a string.
    public object? Role { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class GetProjectDto
{
    public int Id { get; set; }
}

public sealed class GetTeamDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public GetProjectDto? Project { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TeamLeadId { get; set; }
    public GetUserDto? TeamLead { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class GetTaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public GetTeamDto? Team { get; set; }
    public int? AssignedTo { get; set; }
    public GetUserDto? AssignedToUser { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public int? EstimatedHours { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class GetFocusSessionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public GetUserDto? User { get; set; }
    public int? TaskId { get; set; }
    public GetTaskDto? Task { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class CreateFocusSessionDto
{
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus Status { get; set; } = FocusSessionStatus.Active;
    public string? Notes { get; set; }
}

public sealed class UpdateFocusSessionDto
{
    public int? TaskId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public FocusSessionStatus? Status { get; set; }
    public string? Notes { get; set; }
}

