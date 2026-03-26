// Placeholder - full model from Umar/Team Lead
namespace Domain.Entities;
public class Team
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TeamLeadId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
}
