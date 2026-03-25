namespace TaskManager.Web.Configuration;

/// <summary>Backend API base URL (no trailing slash). Must match WebAPI + CORS in backend Program.cs.</summary>
public sealed class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>HTTP dev URL for WebAPI (<c>http://localhost:5125</c> in launchSettings). Use <c>https://localhost:7060</c> only if you run HTTPS and trust the dev cert.</summary>
    public string BaseUrl { get; set; } = "http://localhost:5125";
}
