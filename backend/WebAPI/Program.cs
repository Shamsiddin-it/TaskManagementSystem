var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManagementSystem API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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
        CREATE UNIQUE INDEX IF NOT EXISTS "IX_WorkspaceSettings_EmployerId" ON "WorkspaceSettings" ("EmployerId");
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
        CREATE UNIQUE INDEX IF NOT EXISTS "IX_WorkspaceIntegrations_EmployerId_Key" ON "WorkspaceIntegrations" ("EmployerId", "Key");
        """);
}

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
