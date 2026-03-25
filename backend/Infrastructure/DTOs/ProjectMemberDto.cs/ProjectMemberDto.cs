public class ProjectMemberDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarInitials { get; set; }
    public string? AvatarColor { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
    public MemberAvailability Availability { get; set; }
}
