public class TeamMemberFilter
{
    public int? TeamId { get; set; }
    public int? UserId { get; set; }
    public DevRole? DevRole { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? JoinedFrom { get; set; }
    public DateTime? JoinedTo { get; set; }
}
