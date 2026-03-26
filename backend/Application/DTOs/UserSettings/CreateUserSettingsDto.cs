namespace Application.DTOs;

public class CreateUserSettingsDto
{
    public string UserId { get; set; }
    public bool AutoFocusEnabled { get; set; }
    public bool BlockNotificationsDuringFocus { get; set; }
    public int DefaultFocusDurationMinutes { get; set; } = 25;
    public int DailyGoalHours { get; set; } = 8;
}
