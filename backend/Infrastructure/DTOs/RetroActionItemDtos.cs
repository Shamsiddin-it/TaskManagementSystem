public class InsertRetroActionItemDto
{
    public int RetroId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;
    public DateTime? DueDate { get; set; }
    public int? AssignedToId { get; set; }
}

public class UpdateRetroActionItemDto : InsertRetroActionItemDto
{
    public int Id { get; set; }
}

public class GetRetroActionItemDto
{
    public int Id { get; set; }
    public int RetroId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public ActionItemPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public int? AssignedToId { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
