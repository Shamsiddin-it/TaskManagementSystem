namespace Domain.Models;

public class OrgBudget : BaseEntity
{
    // public Guid Id { get; set; } = Guid.NewGuid();
    public string EmployerId { get; set; }
    public ApplicationUser Employer { get; set; } = null!;
    public string Period { get; set; } = string.Empty;           // "Q4 2024"
    public decimal TotalBudget { get; set; }     // $120,000
    public decimal SpentAmount { get; set; }     // $78,000
    public decimal BurnPercent { get; set; }     // 65%
    public decimal? EstimatedRunwayMonths { get; set; }  // 3.2
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    // public DateTime CreatedAt { get; set; }
    // public DateTime UpdatedAt { get; set; }
}
