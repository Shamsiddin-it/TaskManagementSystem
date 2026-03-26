public class InsertTeamMemberDto
{
    public Guid TeamId { get; set; }
    public string UserId { get; set; } = null!;
    public DevRole DevRole { get; set; }
}

public class UpdateTeamMemberDto : InsertTeamMemberDto
{
    public Guid Id { get; set; }
}

public class GetTeamMemberDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string UserId { get; set; } = null!;
    public DevRole DevRole { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
    public int WeeklyCapacityPct { get; set; }
    public int? FocusScore { get; set; }
    public decimal? ThroughputPtsPerWk { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
