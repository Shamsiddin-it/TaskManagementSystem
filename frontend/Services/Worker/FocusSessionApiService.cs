using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Web.Models.Api;

namespace TaskManager.Web.Services.Worker;

public sealed class FocusSessionApiService(HttpClient http)
{
    public string? LastError { get; private set; }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<GetFocusSessionDto>> GetAllAsync(CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.GetAsync("api/FocusSession", ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"FocusSession GET failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetFocusSessionDto>>>(JsonOptions, ct);
        return body?.Date ?? [];
    }

    public async Task<GetFocusSessionDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.GetAsync($"api/FocusSession/{id}", ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"FocusSession GET by id failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<GetFocusSessionDto>>(JsonOptions, ct);
        return body?.Date;
    }

    public Task<bool> PauseAsync(int id, CancellationToken ct = default) =>
        PatchAndReturnOk($"api/FocusSession/{id}/pause", ct);

    public Task<bool> CompleteAsync(int id, CancellationToken ct = default) =>
        PatchAndReturnOk($"api/FocusSession/{id}/complete", ct);

    private async Task<bool> PatchAndReturnOk(string relativeUrl, CancellationToken ct)
    {
        LastError = null;
        var req = new HttpRequestMessage(new HttpMethod("PATCH"), relativeUrl);
        var response = await http.SendAsync(req, ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"FocusSession PATCH failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return false;
        }

        // Server returns Response<string> with Date or Description; we just need success.
        await response.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptions, ct);
        return true;
    }
}

