namespace Domain.Entities;
public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EmployerId { get; set; }
    public string Status { get; set; } = "active"; // active | paused | completed | archived
    public DateOnly? GlobalDeadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User Employer { get; set; } = null!;
}
