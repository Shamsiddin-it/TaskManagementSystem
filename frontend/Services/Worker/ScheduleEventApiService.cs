using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Web.Models.Api;

namespace TaskManager.Web.Services.Worker;

public sealed class ScheduleEventApiService(HttpClient http)
{
    public string? LastError { get; private set; }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<GetScheduleEventDto>> GetAllAsync(CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.GetAsync("api/ScheduleEvent", ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"ScheduleEvent GET failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetScheduleEventDto>>>(JsonOptions, ct);
        return body?.Date ?? [];
    }

    public async Task<bool> AddAsync(CreateScheduleEventDto dto, CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.PostAsJsonAsync("api/ScheduleEvent", dto, ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"ScheduleEvent POST failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return false;
        }

        await response.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptions, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.DeleteAsync($"api/ScheduleEvent/{id}", ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"ScheduleEvent DELETE failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return false;
        }

        await response.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptions, ct);
        return true;
    }
}

