namespace Domain.Models;
public class TaskTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? DefaultPriority { get; set; }
    public int? DefaultEstimatedHours { get; set; }
    public DateTime CreatedAt { get; set; }
}
