namespace Domain.Models;

public class Badge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty; // e.g. "100_percent_completion"
    public string IconUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
