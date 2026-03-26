namespace Application.DTOs;

public class CreateProjectDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EmployerId { get; set; }
    public DateTime? GlobalDeadline { get; set; }
}
