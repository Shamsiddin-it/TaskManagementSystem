public class UserDirectoryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public OnlineStatus OnlineStatus { get; set; }
    public string? AvatarInitials { get; set; }
    public string? AvatarColor { get; set; }
    public List<string> CurrentProjects { get; set; } = [];
    public List<string> Skills { get; set; } = [];
    public int WorkloadPercent { get; set; }
}
