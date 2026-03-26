using System.Text.Json.Serialization;

namespace TaskManager.Web.Models.Api;

/// <summary>Matches backend <c>Response&lt;T&gt;</c> (payload property is <c>Date</c> on server → JSON <c>date</c>).</summary>
public sealed class ApiResponse<T>
{
    public int StatusCode { get; set; }

    public List<string> Description { get; set; } = [];

    [JsonPropertyName("date")]
    public T? Date { get; set; }
}
