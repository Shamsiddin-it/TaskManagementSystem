namespace Application.DTOs;
public class CreateTaskTemplateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? DefaultPriority { get; set; }
    public int? DefaultEstimatedHours { get; set; }
}
