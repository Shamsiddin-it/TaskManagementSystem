public class ProjectChecklist : BaseEntity
{
    // public Guid Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsBlocked { get; set; }
    public int OrderIndex { get; set; }
    public DateTime? CompletedAt { get; set; }
    // public DateTime CreatedAt { get; set; }
}
