using System.Text.Json;

namespace TaskManager.Web.Services.Auth;

internal static class JwtPayloadParser
{
    public static string? TryGetSub(string jwt)
    {
        try
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2)
                return null;

            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = (payload.Length % 4) switch
            {
                2 => payload + "==",
                3 => payload + "=",
                _ => payload
            };

            var bytes = Convert.FromBase64String(payload);
            using var doc = JsonDocument.Parse(bytes);
            if (doc.RootElement.TryGetProperty("sub", out var sub))
                return sub.GetString();
        }
        catch
        {
            /* malformed token */
        }

        return null;
    }
}
