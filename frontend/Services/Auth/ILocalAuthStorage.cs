namespace TaskManager.Web.Services.Auth;

public interface ILocalAuthStorage
{
    ValueTask<string?> GetTokenAsync();
    ValueTask<string?> GetRoleAsync();
    ValueTask SetAsync(string token, string role);
    ValueTask ClearAsync();
}
