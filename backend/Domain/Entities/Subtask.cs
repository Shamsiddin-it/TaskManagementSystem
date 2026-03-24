namespace Domain.Entities;
public class Subtask
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }

    public Task Task { get; set; } = null!;
}
