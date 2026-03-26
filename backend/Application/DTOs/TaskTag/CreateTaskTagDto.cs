namespace Application.DTOs;
public class CreateTaskTagDto
{
    public int TaskId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? Color { get; set; }
}
