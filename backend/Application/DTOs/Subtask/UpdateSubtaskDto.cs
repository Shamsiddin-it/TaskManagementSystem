namespace Application.DTOs;

public class UpdateSubtaskDto
{
    public string? Content { get; set; }
    public bool? IsCompleted { get; set; }
    public int? OrderIndex { get; set; }
}
