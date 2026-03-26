public class CreateProjectDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectType Type { get; set; }
    public DateTime? GlobalDeadline { get; set; }
    public decimal? BudgetAllocated { get; set; }
}
