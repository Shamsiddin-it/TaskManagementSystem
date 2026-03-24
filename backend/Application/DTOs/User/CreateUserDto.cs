using Domain.Enums;

namespace Application.DTOs;
public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Worker;
    public string? AvatarUrl { get; set; }
}
