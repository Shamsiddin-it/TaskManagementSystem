public class TimelinePhaseDto
{
    public Guid Id { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ColorHex { get; set; } = string.Empty;
    public GanttPhaseStatus Status { get; set; }
    public int OrderIndex { get; set; }
}
