public class Project
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid EmployerId { get; set; }
    public User Employer { get; set; } = null!;
    public ProjectStatus Status { get; set; }   // planning | active | at_risk | paused | completed | archived
    public ProjectType Type { get; set; }        // enterprise | mobile | api | web | internal | rd | marketing | security | fintech | core | devops | uiux
    public DateTime? GlobalDeadline { get; set; }
    public decimal? BudgetAllocated { get; set; }  // из формы создания проекта
    public decimal? BudgetSpent { get; set; }
    public int CompletionPercent { get; set; }     // 0–100, для прогресс бара
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Team> Teams { get; set; } = [];
    public ICollection<ProjectMember> Members { get; set; } = [];
    public ICollection<ProjectRisk> Risks { get; set; } = [];
    public ICollection<ProjectComment> Comments { get; set; } = [];
}
