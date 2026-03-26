public class InsertRetroActionItemDto
{
    public Guid RetroId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;
    public DateTime? DueDate { get; set; }
    public string? AssignedToId { get; set; }
}

public class UpdateRetroActionItemDto : InsertRetroActionItemDto
{
    public Guid Id { get; set; }
}

public class GetRetroActionItemDto
{
    public Guid Id { get; set; }
    public Guid RetroId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public ActionItemPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? AssignedToId { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
