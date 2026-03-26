namespace Application.DTOs;

public class GetProjectDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EmployerId { get; set; }
    public GetUserDto Employer { get; set; } = null!;
    public string Status { get; set; } = string.Empty;
    public DateTime? GlobalDeadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
