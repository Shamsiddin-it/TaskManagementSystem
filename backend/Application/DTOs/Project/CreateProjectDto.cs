namespace Application.DTOs;

public class CreateProjectDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EmployerId { get; set; }
    public DateOnly? GlobalDeadline { get; set; }
}
