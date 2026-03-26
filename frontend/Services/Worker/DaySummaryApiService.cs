using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Web.Models.Api;

namespace TaskManager.Web.Services.Worker;

public sealed class DaySummaryApiService(HttpClient http)
{
    public string? LastError { get; private set; }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<GetDaySummaryDto>> GetAllAsync(CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.GetAsync("api/DaySummary", ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"DaySummary GET failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetDaySummaryDto>>>(JsonOptions, ct);
        return body?.Date ?? [];
    }

    public async Task<bool> AddAsync(CreateDaySummaryDto dto, CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.PostAsJsonAsync("api/DaySummary", dto, ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"DaySummary POST failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return false;
        }

        await response.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptions, ct);
        return true;
    }

    public async Task<bool> UpdateAsync(int id, UpdateDaySummaryDto dto, CancellationToken ct = default)
    {
        LastError = null;
        var response = await http.PutAsJsonAsync($"api/DaySummary/{id}", dto, ct);
        if (!response.IsSuccessStatusCode)
        {
            LastError = $"DaySummary PUT failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return false;
        }

        await response.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptions, ct);
        return true;
    }
}

