namespace Domain.Entities;

public class ActivityLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? TaskId { get; set; }
    public string ActionType { get; set; } = string.Empty; // timer_started, status_changed, file_added, task_created, comment_added
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Task? Task { get; set; }
}
