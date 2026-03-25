public class ProjectMemberFilter
{
    public Guid? Id { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? UserId { get; set; }
    public string? ProjectRole { get; set; }
    public MemberAvailability? Availability { get; set; }
    public DateTime? JoinedFrom { get; set; }
    public DateTime? JoinedTo { get; set; }
}
