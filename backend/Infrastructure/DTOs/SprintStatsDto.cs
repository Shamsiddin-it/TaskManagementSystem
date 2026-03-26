public class SprintStatsDto
{
    public Guid SprintId { get; set; }
    public int PlannedPoints { get; set; }
    public int CompletedPoints { get; set; }
    public int SpilloverPoints { get; set; }
    public decimal Velocity { get; set; }
    public List<int> BurndownPoints { get; set; } = new();
    public Dictionary<int, int> PointsByMember { get; set; } = new();
}
