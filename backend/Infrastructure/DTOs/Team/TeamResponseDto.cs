public class TeamResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public TeamLeadDto? TeamLead { get; set; }
    public int MemberCount { get; set; }
    public int CompletionPercent { get; set; }
}
