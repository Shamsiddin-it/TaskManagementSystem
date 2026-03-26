using System.Net.Http.Headers;
using TaskManager.Web.Services.Auth;

namespace TaskManager.Web.Services.Http;

public sealed class AuthHeaderHandler(ILocalAuthStorage storage) : DelegatingHandler
{
    private readonly ILocalAuthStorage _storage = storage;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;
        if (!path.Contains("/api/Auth/login", StringComparison.OrdinalIgnoreCase)
            && !path.Contains("/api/Auth/register", StringComparison.OrdinalIgnoreCase))
        {
            var token = await _storage.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
