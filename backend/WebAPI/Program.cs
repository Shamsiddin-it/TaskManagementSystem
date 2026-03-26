using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.EmailService;
using WebApi.Entities;
using WebApi.Seeds;
using System.Linq;
using Application;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Domain.Models;

var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
// builder.Services.Configure<GeminiSettings>(
//     builder.Configuration.GetSection("Gemini"));
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();
builder.Services.AddScoped<IProjectRiskService, ProjectRiskService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ITimelineService, TimelineService>();
builder.Services.AddScoped<IEmployerNotificationService, EmployerNotificationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();

builder.Services.AddControllers();
builder.Services.AddScoped<IJwtTokenService,JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IRetroActionItemService, RetroActionItemService>();
builder.Services.AddScoped<ISprintService, SprintService>();
builder.Services.AddScoped<ISprintRetroService, SprintRetroService>();
builder.Services.AddScoped<ISubtaskService, SubtaskService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskTagService, TaskTagService>();
builder.Services.AddScoped<ITeamMemberService, TeamMemberService>();
builder.Services.AddScoped<ITicketCodeGenerator, TicketCodeGenerator>();
// builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(InfrastructureProfile));
builder.Services.AddLogging(config => {config.AddConsole(); config.SetMinimumLevel(LogLevel.Information);});
var jwt = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddSwaggerGen(options=>{
     options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен так: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build(); 

try
{
    await using var scope = app.Services.CreateAsyncScope();
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var db = services.GetRequiredService<ApplicationDbContext>();

    await DefaultRoles.SeedRoles(roleManager);
    
    // Ensure tables from the other branch exist (if not handled by migrations)
    await db.Database.ExecuteSqlRawAsync("""
        CREATE TABLE IF NOT EXISTS "WorkspaceSettings" (
            "Id" uuid PRIMARY KEY,
            "EmployerId" uuid NOT NULL,
            "OrganizationName" character varying(200) NOT NULL,
            "OrganizationCode" character varying(50) NOT NULL,
            "PrimaryContactName" character varying(150) NOT NULL,
            "ContactEmailAddress" character varying(200) NOT NULL,
            "CompanyWebsite" character varying(300) NOT NULL,
            "Industry" character varying(100) NOT NULL,
            "CompanySize" character varying(100) NOT NULL,
            "PlanName" character varying(50) NOT NULL,
            "PlanPriceMonthly" numeric(18,2) NOT NULL,
            "TeamMembersLimit" integer NOT NULL,
            "ActiveProjectsLimit" integer NOT NULL,
            "NextBillingDate" timestamp with time zone NOT NULL,
            "PaymentMethodLast4" character varying(4) NOT NULL,
            "BillingEmail" character varying(200) NOT NULL,
            "TaxIdOrVatNumber" character varying(100) NOT NULL,
            "DefaultTeamSizeLimit" integer NOT NULL,
            "DefaultPtoPolicy" character varying(100) NOT NULL,
            "DefaultWorkSchedule" character varying(100) NOT NULL,
            "PrimaryTimezone" character varying(100) NOT NULL,
            "AutoProvisionNewHires" boolean NOT NULL,
            "RequireManagerApprovalForTimeOff" boolean NOT NULL,
            "RequireTwoFactorAuthentication" boolean NOT NULL,
            "EnforceIpAllowlist" boolean NOT NULL,
            "DataEncryptionAtRest" boolean NOT NULL,
            "IdleSessionTimeout" character varying(50) NOT NULL,
            "AuditLogRetention" character varying(50) NOT NULL,
            "SsoProviderName" character varying(100) NOT NULL,
            "SsoConnected" boolean NOT NULL,
            "CreatedAt" timestamp with time zone NOT NULL,
            "UpdatedAt" timestamp with time zone NOT NULL
        );
        """);

    await db.Database.ExecuteSqlRawAsync("""
        CREATE TABLE IF NOT EXISTS "WorkspaceIntegrations" (
            "Id" uuid PRIMARY KEY,
            "EmployerId" uuid NOT NULL,
            "Key" character varying(50) NOT NULL,
            "Name" character varying(150) NOT NULL,
            "Status" character varying(50) NOT NULL,
            "IsConnected" boolean NOT NULL,
            "Accent" character varying(50) NOT NULL,
            "CreatedAt" timestamp with time zone NOT NULL,
            "UpdatedAt" timestamp with time zone NOT NULL
        );
        """);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during startup.");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestTimeMiddleware>();
app.UseCors("AllowAll");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
