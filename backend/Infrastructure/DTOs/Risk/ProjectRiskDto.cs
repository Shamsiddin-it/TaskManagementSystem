public class ProjectRiskDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public RiskSeverity Severity { get; set; }
    public RiskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
