namespace Application.DTOs;

public class GetProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EmployerId { get; set; }
    public GetUserDto Employer { get; set; } = null!;
    public string Status { get; set; } = string.Empty;
    public DateOnly? GlobalDeadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
