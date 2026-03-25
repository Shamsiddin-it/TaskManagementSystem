public class ProjectRisk
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public RiskSeverity Severity { get; set; }   // low | medium | high | critical
    public RiskStatus Status { get; set; }        // open | mitigated | resolved
    public DateTime CreatedAt { get; set; }
}
