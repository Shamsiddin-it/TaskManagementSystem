namespace Application.DTOs;

public class GetUserSettingsDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public bool AutoFocusEnabled { get; set; }
    public bool BlockNotificationsDuringFocus { get; set; }
    public int DefaultFocusDurationMinutes { get; set; }
    public int DailyGoalHours { get; set; }
    public DateTime UpdatedAt { get; set; }
}
