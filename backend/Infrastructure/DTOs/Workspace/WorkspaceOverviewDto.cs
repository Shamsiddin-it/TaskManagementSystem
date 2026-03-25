public class WorkspaceOverviewDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int ActiveMembers { get; set; }
    public decimal MonthlyCost { get; set; }
    public int CompletionRate { get; set; }
    public int UnreadNotifications { get; set; }
}
