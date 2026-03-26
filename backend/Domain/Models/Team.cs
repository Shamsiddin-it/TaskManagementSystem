namespace Domain.Models;

public class Team : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? TeamLeadId { get; set; }
    public ApplicationUser? TeamLead { get; set; }
    public string? Description { get; set; }
}
