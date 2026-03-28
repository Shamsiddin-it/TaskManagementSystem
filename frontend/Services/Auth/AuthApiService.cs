using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Web.Models.Api;

namespace TaskManager.Web.Services.Auth;

// Matches the flat AuthResponseDto returned directly by the backend AuthController.
internal sealed class FlatAuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;
}

public sealed class AuthApiService(HttpClient http, ILocalAuthStorage storage, NexusAuthStateProvider authState)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http = http;
    private readonly ILocalAuthStorage _storage = storage;
    private readonly NexusAuthStateProvider _authState = authState;

    public async Task<(bool Ok, string? ErrorMessage, string? Role)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", new LoginRequest { Email = email, Password = password }, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errMsg = $"HTTP {(int)response.StatusCode}";
            try
            {
                var errBody = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, ct);
                if (errBody.TryGetProperty("message", out var m)) errMsg = m.GetString() ?? errMsg;
            }
            catch { /* ignore parse errors */ }
            return (false, errMsg, null);
        }

        // Backend returns a flat AuthResponseDto — NOT wrapped in ApiResponse<T>
        var body = await response.Content.ReadFromJsonAsync<FlatAuthResponse>(JsonOptions, ct);
        if (body is { Token.Length: > 0 } payload)
        {
            await _storage.SetAsync(payload.Token, payload.Role);
            _authState.NotifyUserChanged();
            return (true, null, payload.Role);
        }

        return (false, "Unexpected response from server.", null);
    }

    public async Task<(bool Ok, string? ErrorMessage, string? Role)> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/register", request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errMsg = $"HTTP {(int)response.StatusCode}";
            try
            {
                var errBody = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, ct);
                if (errBody.TryGetProperty("message", out var m)) errMsg = m.GetString() ?? errMsg;
            }
            catch { /* ignore parse errors */ }
            return (false, errMsg, null);
        }

        // Backend returns a flat AuthResponseDto — NOT wrapped in ApiResponse<T>
        var body = await response.Content.ReadFromJsonAsync<FlatAuthResponse>(JsonOptions, ct);
        if (body is { Token.Length: > 0 } payload)
        {
            await _storage.SetAsync(payload.Token, payload.Role);
            _authState.NotifyUserChanged();
            return (true, null, payload.Role);
        }

        return (false, "Unexpected response from server.", null);
    }

    public async Task LogoutAsync()
    {
        await _storage.ClearAsync();
        _authState.NotifyUserChanged();
    }
}
