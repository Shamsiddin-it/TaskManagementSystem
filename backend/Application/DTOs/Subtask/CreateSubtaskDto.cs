namespace Application.DTOs;

public class CreateSubtaskDto
{
    public int TaskId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int OrderIndex { get; set; }
}
