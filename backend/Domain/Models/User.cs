public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }        // employer | teamlead | worker
    public string? AvatarUrl { get; set; }
    public string? AvatarInitials { get; set; }  // JD, AK, MR — для отображения
    public string? AvatarColor { get; set; }     // цвет аватара (#58A6FF, #3FB950...)
    public OnlineStatus OnlineStatus { get; set; } // online | idle | offline | in_meeting | dnd | out_of_office
    public DateTime? LastActiveAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
