namespace Application.DTOs;
public class UpdateTaskTemplateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? DefaultPriority { get; set; }
    public int? DefaultEstimatedHours { get; set; }
}
