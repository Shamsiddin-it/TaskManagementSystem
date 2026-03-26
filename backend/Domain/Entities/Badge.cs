namespace Domain.Entities;

public class Badge
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty; // e.g. "100_percent_completion"
    public string IconUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
