public class InsertSprintRetroDto
{
    public int SprintId { get; set; }
    public int CreatedById { get; set; }
    public string? WentWell { get; set; }
    public string? BlockedSummary { get; set; }
    public string? Notes { get; set; }
}

public class UpdateSprintRetroDto : InsertSprintRetroDto
{
    public int Id { get; set; }
}

public class GetSprintRetroDto
{
    public int Id { get; set; }
    public int SprintId { get; set; }
    public int CreatedById { get; set; }
    public int PlannedPoints { get; set; }
    public int CompletedPoints { get; set; }
    public int SpilloverPoints { get; set; }
    public string? WentWell { get; set; }
    public string? BlockedSummary { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
