using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TaskManager.Web.Services.Auth;

public sealed class NexusAuthStateProvider(ILocalAuthStorage storage) : AuthenticationStateProvider
{
    private readonly ILocalAuthStorage _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _storage.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonymous();

        var role = await _storage.GetRoleAsync() ?? "worker";
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role)
        };

        var sub = JwtPayloadParser.TryGetSub(token);
        if (!string.IsNullOrEmpty(sub))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));

        var identity = new ClaimsIdentity(claims, authenticationType: "NexusJwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static AuthenticationState Anonymous() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));
}
