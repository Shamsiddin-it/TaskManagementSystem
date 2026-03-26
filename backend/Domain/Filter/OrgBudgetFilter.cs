public class OrgBudgetFilter
{
    public Guid? Id { get; set; }
    public Guid? EmployerId { get; set; }
    public string? Period { get; set; }
    public decimal? TotalBudgetFrom { get; set; }
    public decimal? TotalBudgetTo { get; set; }
    public DateTime? PeriodStartFrom { get; set; }
    public DateTime? PeriodEndTo { get; set; }
}
