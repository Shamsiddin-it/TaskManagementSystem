namespace Application.DTOs;
public class GetTaskTagDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public GetTaskDto Task { get; set; } = null!;
    public string TagName { get; set; } = string.Empty;
    public string? Color { get; set; }
}
