using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Web.Models.Api;

namespace TaskManager.Web.Services.Worker;

public sealed class UserSettingsApiService(HttpClient http)
{
    public string? LastError { get; private set; }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<GetUserSettingsDto?> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.GetAsync($"api/UserSettings/user/{userId}", ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"UserSettings GET failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<GetUserSettingsDto>>(JsonOptions, ct);
        return body?.Date;
    }

    public async Task<bool> AddAsync(CreateUserSettingsDto dto, CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.PostAsJsonAsync("api/UserSettings", dto, ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"UserSettings POST failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return false;
        }

        await response.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptions, ct);
        return true;
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserSettingsDto dto, CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.PutAsJsonAsync($"api/UserSettings/{id}", dto, ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"UserSettings PUT failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return false;
        }

        await response.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptions, ct);
        return true;
    }
}

