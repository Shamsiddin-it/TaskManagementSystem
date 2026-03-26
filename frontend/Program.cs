using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskManager.Web;
using TaskManager.Web.Configuration;
using TaskManager.Web.Services.Auth;
using TaskManager.Web.Services.Http;
using TaskManager.Web.Services.Worker;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBase = builder.Configuration
    .GetSection(ApiOptions.SectionName)
    .Get<ApiOptions>()?
    .BaseUrl
    ?.TrimEnd('/')
    ?? "http://localhost:5125";

var apiUri = new Uri(apiBase + "/");

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<ILocalAuthStorage, LocalAuthStorage>();
builder.Services.AddScoped<NexusAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<NexusAuthStateProvider>());
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<AuthHeaderHandler>();

// Single HttpClient for the whole app, with JWT automatically attached
// (except for login/register where token is not yet available).
builder.Services.AddScoped(sp =>
{
    var handler = new AuthHeaderHandler(sp.GetRequiredService<ILocalAuthStorage>())
    {
        InnerHandler = new HttpClientHandler()
    };

    return new HttpClient(handler) { BaseAddress = apiUri };
});

builder.Services.AddScoped<DaySummaryApiService>();
builder.Services.AddScoped<FocusSessionApiService>();
builder.Services.AddScoped<NotificationApiService>();
builder.Services.AddScoped<UserSettingsApiService>();
builder.Services.AddScoped<ScheduleEventApiService>();

await builder.Build().RunAsync();
