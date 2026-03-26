public class InsertSubtaskDto
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = null!;
}

public class UpdateSubtaskDto : InsertSubtaskDto
{
    public Guid Id { get; set; }
}

public class GetSubtaskDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public int OrderIndex { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
