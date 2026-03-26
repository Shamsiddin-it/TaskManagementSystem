using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Web.Models.Api;

namespace TaskManager.Web.Services.Worker;

public sealed class NotificationApiService(HttpClient http)
{
    public string? LastError { get; private set; }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<GetNotificationDto>> GetAllAsync(CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.GetAsync("api/Notification", ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"Notification GET failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetNotificationDto>>>(JsonOptions, ct);
        return body?.Date ?? [];
    }

    public async Task<bool> MarkAsReadAsync(int id, CancellationToken ct = default)
    {
        LastError = null;
        var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/Notification/{id}/read");
        var response = await http.SendAsync(req, ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"Notification mark read failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return false;
        }

        await response.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptions, ct);
        return true;
    }
}

