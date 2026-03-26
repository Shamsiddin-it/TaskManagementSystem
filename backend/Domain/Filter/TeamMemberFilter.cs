namespace Domain.Models;

public class TeamMemberFilter
{
    public Guid? Id { get; set; }
    public Guid? TeamId { get; set; }
    public string? UserId { get; set; }
    public bool? IsActive { get; set; }
    public DevRole? DevRole { get; set; }
    public DateTime? JoinedFrom { get; set; }
    public DateTime? JoinedTo { get; set; }
}
