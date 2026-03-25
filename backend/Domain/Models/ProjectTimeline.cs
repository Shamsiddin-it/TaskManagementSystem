public class ProjectTimeline
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string PhaseName { get; set; } = string.Empty;       // "UI/UX Design", "API Integration"...
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ColorHex { get; set; } = string.Empty;        
    public int OrderIndex { get; set; }
    public GanttPhaseStatus Status { get; set; }  // not_started | in_progress |- completed | blocked
}
