public class ProjectStatsDto
{
    public int ProjectId { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int BlockedTasks { get; set; }
    public int TotalMembers { get; set; }
    public int CompletionPercent { get; set; }
    public decimal BudgetBurnPercent { get; set; }
    public List<ProjectRiskDto> Risks { get; set; } = [];
}
