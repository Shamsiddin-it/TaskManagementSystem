public class OrgBudget
{
    public Guid Id { get; set; }
    public Guid EmployerId { get; set; }
    public User Employer { get; set; } = null!;
    public string Period { get; set; } = string.Empty;           // "Q4 2024"
    public decimal TotalBudget { get; set; }     // $120,000
    public decimal SpentAmount { get; set; }     // $78,000
    public decimal BurnPercent { get; set; }     // 65%
    public decimal? EstimatedRunwayMonths { get; set; }  // 3.2
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
