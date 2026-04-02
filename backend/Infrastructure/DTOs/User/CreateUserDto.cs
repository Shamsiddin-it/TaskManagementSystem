using Domain.Enums;

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Worker;
    public OnlineStatus OnlineStatus { get; set; } = OnlineStatus.Online;
    public List<string> Skills { get; set; } = [];
}
