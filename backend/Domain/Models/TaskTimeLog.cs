using Domain.Models;

public class TaskTimeLog : BaseEntity
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public int Minutes { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public Task? Task { get; set; }
    public ApplicationUser? User { get; set; }
}
