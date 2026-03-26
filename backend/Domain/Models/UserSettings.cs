namespace Domain.Models;
public class UserSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public bool AutoFocusEnabled { get; set; }
    public bool BlockNotificationsDuringFocus { get; set; }
    public int DefaultFocusDurationMinutes { get; set; } = 25;
    public int DailyGoalHours { get; set; } = 8;
    public DateTime UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
