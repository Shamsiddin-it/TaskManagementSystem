using System.Text.Json.Serialization;

namespace TaskManager.Web.Models.Api;

public sealed class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// <summary>0 = Employer, 1 = TeamLead, 2 = Worker (matches backend <c>UserRole</c> enum order).</summary>
    public int Role { get; set; } = 2;
}

public sealed class AuthPayloadDto
{
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expiresAt")]
    public DateTime ExpiresAt { get; set; }
}
