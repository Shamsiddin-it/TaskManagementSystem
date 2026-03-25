public class InsertSubtaskDto
{
    public int TaskId { get; set; }
    public string Title { get; set; } = null!;
}

public class UpdateSubtaskDto : InsertSubtaskDto
{
    public int Id { get; set; }
}

public class GetSubtaskDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public int OrderIndex { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
