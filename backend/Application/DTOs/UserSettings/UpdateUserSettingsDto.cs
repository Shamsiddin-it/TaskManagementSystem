namespace Application.DTOs;

public class UpdateUserSettingsDto
{
    public bool? AutoFocusEnabled { get; set; }
    public bool? BlockNotificationsDuringFocus { get; set; }
    public int? DefaultFocusDurationMinutes { get; set; }
    public int? DailyGoalHours { get; set; }
}
