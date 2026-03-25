public class OrgBudgetDto
{
    public string Period { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal BurnPercent { get; set; }
    public decimal? EstimatedRunwayMonths { get; set; }
}
