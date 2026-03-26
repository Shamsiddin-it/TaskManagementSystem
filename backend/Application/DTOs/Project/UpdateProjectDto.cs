namespace Application.DTOs;

public class UpdateProjectDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public DateTime? GlobalDeadline { get; set; }
}
