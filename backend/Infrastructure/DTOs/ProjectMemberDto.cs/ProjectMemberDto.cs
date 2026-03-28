public class ProjectMemberDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarInitials { get; set; }
    public string? AvatarColor { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
    public MemberAvailability Availability { get; set; }
}
