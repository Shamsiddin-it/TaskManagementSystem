public class ReportsDashboardDto
{
    public List<ReportsTrendPointDto> CompletionTrend { get; set; } = [];
    public List<ProjectSpendSummaryDto> BudgetSpendByProject { get; set; } = [];
    public List<int> TeamHeatmap { get; set; } = [];
}

public class ReportsTrendPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Actual { get; set; }
    public decimal Target { get; set; }
}

public class ProjectSpendSummaryDto
{
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
