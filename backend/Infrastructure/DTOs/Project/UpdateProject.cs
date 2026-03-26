public class UpdateProjectDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? Status { get; set; }
    public ProjectType? Type { get; set; }
    public DateTime? GlobalDeadline { get; set; }
    public decimal? BudgetAllocated { get; set; }
}