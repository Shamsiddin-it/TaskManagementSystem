using Microsoft.JSInterop;

namespace TaskManager.Web.Services.Auth;

public sealed class LocalAuthStorage(IJSRuntime js) : ILocalAuthStorage
{
    private readonly IJSRuntime _js = js;

    public ValueTask<string?> GetTokenAsync() =>
        _js.InvokeAsync<string?>("nexusAuthStorage.getToken");

    public ValueTask<string?> GetRoleAsync() =>
        _js.InvokeAsync<string?>("nexusAuthStorage.getRole");

    public async ValueTask SetAsync(string token, string role)
    {
        await _js.InvokeVoidAsync("nexusAuthStorage.setToken", token);
        await _js.InvokeVoidAsync("nexusAuthStorage.setRole", role);
    }

    public ValueTask ClearAsync() =>
        _js.InvokeVoidAsync("nexusAuthStorage.clearToken");
}
