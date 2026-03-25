using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
public class RequestTimeMiddleware(RequestDelegate next, ILogger<RequestTimeMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestTimeMiddleware> _logger = logger;
    public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation(
            "Incoming request: {Method} {Path}",
            context.Request.Method,
            context.Request.Path
        );
        var start = DateTime.Now;
        try
        {
            await _next(context);
        }
        catch
        {
            _logger.LogError("The request was not successful");
        }
        var end = DateTime.Now;
        _logger.LogInformation("The request was finished!");
        Console.WriteLine($"Request took {(end - start).TotalMilliseconds} ms");
    }
}
