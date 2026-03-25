public class ProjectRiskFilter
{
    public Guid? Id { get; set; }
    public Guid? ProjectId { get; set; }
    public string? Description { get; set; }
    public RiskSeverity? Severity { get; set; }
    public RiskStatus? Status { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
