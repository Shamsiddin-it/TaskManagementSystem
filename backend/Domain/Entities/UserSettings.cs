namespace Domain.Entities;
public class UserSettings
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool AutoFocusEnabled { get; set; }
    public bool BlockNotificationsDuringFocus { get; set; }
    public int DefaultFocusDurationMinutes { get; set; } = 25;
    public int DailyGoalHours { get; set; } = 8;
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
