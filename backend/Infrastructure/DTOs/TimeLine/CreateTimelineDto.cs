public class CreateTimelinePhaseDto
{
    public string PhaseName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ColorHex { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}
