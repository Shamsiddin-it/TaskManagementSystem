namespace Application.DTOs;
public class GetSubtaskDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public GetTaskDto Task { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
}
