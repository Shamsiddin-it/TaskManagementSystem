public class InsertTeamMemberDto
{
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public DevRole DevRole { get; set; }
}

public class UpdateTeamMemberDto : InsertTeamMemberDto
{
    public int Id { get; set; }
}

public class GetTeamMemberDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public DevRole DevRole { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
    public int WeeklyCapacityPct { get; set; }
    public int? FocusScore { get; set; }
    public decimal? ThroughputPtsPerWk { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
