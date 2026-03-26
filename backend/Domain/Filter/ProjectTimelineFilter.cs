public class ProjectTimelineFilter
{
    public Guid? Id { get; set; }
    public Guid? ProjectId { get; set; }
    public string? PhaseName { get; set; }
    public GanttPhaseStatus? Status { get; set; }
    public DateTime? StartFrom { get; set; }
    public DateTime? StartTo { get; set; }
    public DateTime? EndFrom { get; set; }
    public DateTime? EndTo { get; set; }
}
