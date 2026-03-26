using Domain.Models;

public class TaskComment : BaseEntity
{
    public int TaskId { get; set; }
    public string UserId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public Task? Task { get; set; }
    public ApplicationUser? User { get; set; }
}
