public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public ProjectType Type { get; set; }
    public DateTime? GlobalDeadline { get; set; }
    public decimal? BudgetAllocated { get; set; }
    public decimal? BudgetSpent { get; set; }
    public int CompletionPercent { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TeamSummaryDto> Teams { get; set; } = [];
    public List<ProjectMemberDto> Members { get; set; } = [];
}
