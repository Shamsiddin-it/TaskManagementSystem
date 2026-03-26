public class InsertSprintRetroDto
{
    public Guid SprintId { get; set; }
    public string CreatedById { get; set; } = null!;
    public string? WentWell { get; set; }
    public string? BlockedSummary { get; set; }
    public string? Notes { get; set; }
}

public class UpdateSprintRetroDto : InsertSprintRetroDto
{
    public Guid Id { get; set; }
}

public class GetSprintRetroDto
{
    public Guid Id { get; set; }
    public Guid SprintId { get; set; }
    public string CreatedById { get; set; } = null!;
    public int PlannedPoints { get; set; }
    public int CompletedPoints { get; set; }
    public int SpilloverPoints { get; set; }
    public string? WentWell { get; set; }
    public string? BlockedSummary { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
