using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Worker;
    public string? AvatarUrl { get; set; }
    public string? AvatarInitials { get; set; }
    public string? AvatarColor { get; set; }
    public OnlineStatus OnlineStatus { get; set; } = OnlineStatus.Offline;
    public DateTime? LastActiveAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(static part => !string.IsNullOrWhiteSpace(part)));
}
