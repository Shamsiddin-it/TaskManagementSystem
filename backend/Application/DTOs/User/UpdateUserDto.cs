using Domain.Enums;

namespace Application.DTOs;

public class UpdateUserDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public UserRole? Role { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? IsActive { get; set; }
}
