public class InsertSprintDto
{
    public int TeamId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CapacityPoints { get; set; } = 40;
    public string? Goal { get; set; }
}

public class UpdateSprintDto : InsertSprintDto
{
    public int Id { get; set; }
}

public class GetSprintDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string Name { get; set; } = null!;
    public int Number { get; set; }
    public SprintStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPoints { get; set; }
    public int CompletedPoints { get; set; }
    public int CapacityPoints { get; set; }
    public string? Goal { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
