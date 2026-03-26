public class TaskComment : BaseEntity
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; } = null!;
    public Task? Task { get; set; }
    public ApplicationUser? User { get; set; }
}
