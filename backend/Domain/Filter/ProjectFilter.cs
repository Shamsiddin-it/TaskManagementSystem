public class ProjectFilter
{
    public Guid? Id { get; set; }
    public Guid? EmployerId { get; set; }
    public string? Title { get; set; }
    public ProjectStatus? Status { get; set; }
    public ProjectType? Type { get; set; }
    public DateTime? DeadlineFrom { get; set; }
    public DateTime? DeadlineTo { get; set; }
    public decimal? BudgetAllocatedFrom { get; set; }
    public decimal? BudgetAllocatedTo { get; set; }
}
