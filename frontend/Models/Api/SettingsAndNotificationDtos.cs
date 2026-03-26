namespace TaskManager.Web.Models.Api;

public enum NotificationType
{
    // Matches backend enum names; System.Text.Json can deserialize numeric or string.
    TaskAssigned = 0,
    TaskDeadline = 1,
    NewComment = 2,
    InvitationReceived = 3,
    AbsenceApproved = 4,
    MemberRemoved = 5
}

public sealed class GetNotificationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int? RelatedId { get; set; }
    public string? RelatedType { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class GetUserSettingsDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool AutoFocusEnabled { get; set; }
    public bool BlockNotificationsDuringFocus { get; set; }
    public int DefaultFocusDurationMinutes { get; set; }
    public int DailyGoalHours { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class UpdateUserSettingsDto
{
    public bool? AutoFocusEnabled { get; set; }
    public bool? BlockNotificationsDuringFocus { get; set; }
    public int? DefaultFocusDurationMinutes { get; set; }
    public int? DailyGoalHours { get; set; }
}

public sealed class CreateUserSettingsDto
{
    public int UserId { get; set; }
    public bool AutoFocusEnabled { get; set; }
    public bool BlockNotificationsDuringFocus { get; set; }
    public int DefaultFocusDurationMinutes { get; set; } = 25;
    public int DailyGoalHours { get; set; } = 8;
}

