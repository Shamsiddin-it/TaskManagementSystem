namespace Application.DTOs;
public class GetTaskTagDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public GetTaskDto Task { get; set; } = null!;
    public string TagName { get; set; } = string.Empty;
    public string? Color { get; set; }
}
