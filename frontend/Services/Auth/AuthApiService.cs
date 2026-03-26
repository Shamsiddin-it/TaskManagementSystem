using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Web.Models.Api;

namespace TaskManager.Web.Services.Auth;

public sealed class AuthApiService(HttpClient http, ILocalAuthStorage storage, NexusAuthStateProvider authState)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http = http;
    private readonly ILocalAuthStorage _storage = storage;
    private readonly NexusAuthStateProvider _authState = authState;

    public async Task<(bool Ok, string? ErrorMessage)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", new LoginRequest { Email = email, Password = password }, ct);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthPayloadDto>>(JsonOptions, ct);

        if (body?.Date is { Token: { Length: > 0 } token } payload)
        {
            await _storage.SetAsync(token, payload.Role);
            _authState.NotifyUserChanged();
            return (true, null);
        }

        var msg = body?.Description.FirstOrDefault() ?? $"HTTP {(int)response.StatusCode}";
        return (false, msg);
    }

    public async Task<(bool Ok, string? ErrorMessage)> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/register", request, ct);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthPayloadDto>>(JsonOptions, ct);

        if (response.IsSuccessStatusCode && body?.Date is { Token: { Length: > 0 } token } payload)
        {
            await _storage.SetAsync(token, payload.Role);
            _authState.NotifyUserChanged();
            return (true, null);
        }

        var msg = body?.Description.FirstOrDefault() ?? $"HTTP {(int)response.StatusCode}";
        return (false, msg);
    }

    public async Task LogoutAsync()
    {
        await _storage.ClearAsync();
        _authState.NotifyUserChanged();
    }
}
