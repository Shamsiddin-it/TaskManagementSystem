namespace Domain.Entities;
public class TaskTag
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TagName { get; set; } = string.Empty; // HIGH PRIORITY, BACKEND, SPRINT 12
    public string? Color { get; set; }

    public Task Task { get; set; } = null!;
}
