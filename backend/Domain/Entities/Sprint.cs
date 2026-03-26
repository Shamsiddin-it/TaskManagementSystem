namespace Domain.Entities;
public class Sprint
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = "active"; // active | completed | planned
    public DateTime CreatedAt { get; set; }

    public Team Team { get; set; } = null!;
}
