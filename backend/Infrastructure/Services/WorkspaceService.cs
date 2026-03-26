using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class WorkspaceService : IWorkspaceService
{
    private readonly ApplicationDbContext _db;

    public WorkspaceService(ApplicationDbContext db) => _db = db;

    public async Task<Response<WorkspaceOverviewDto>> GetOverviewAsync(string employerId)
    {
        try
        {
            var projects = await _db.Projects.Where(p => p.EmployerId == employerId).ToListAsync();
            var projectIds = projects.Select(p => p.Id).ToList();

            var activeMembers = await _db.ProjectMembers
                .Where(m => projectIds.Contains(m.ProjectId))
                .Select(m => m.UserId)
                .Distinct()
                .CountAsync();

            var monthlyCost = await _db.BudgetRecords
                .Where(r => projectIds.Contains(r.ProjectId)
                    && r.Type == BudgetRecordType.Expense
                    && r.RecordDate.Year == DateTime.UtcNow.Year
                    && r.RecordDate.Month == DateTime.UtcNow.Month)
                .SumAsync(r => (decimal?)r.Amount) ?? 0;

            var unreadNotifications = await _db.EmployerNotifications
                .CountAsync(n => n.EmployerId == employerId && !n.IsRead);

            var completionRate = projects.Count == 0
                ? 0
                : (int)Math.Round(projects.Average(p => p.CompletionPercent));

            return new Response<WorkspaceOverviewDto>(
                HttpStatusCode.OK,
                "Workspace overview retrieved successfully",
                new WorkspaceOverviewDto
                {
                    TotalProjects = projects.Count,
                    ActiveProjects = projects.Count(p => p.Status is ProjectStatus.Active or ProjectStatus.AtRisk),
                    ActiveMembers = activeMembers,
                    MonthlyCost = monthlyCost,
                    CompletionRate = completionRate,
                    UnreadNotifications = unreadNotifications
                });
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceOverviewDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<WorkspaceSettingsDto>> GetSettingsAsync(string employerId)
    {
        try
        {
            var dto = await BuildSettingsDtoAsync(employerId);
            return new Response<WorkspaceSettingsDto>(HttpStatusCode.OK, "Workspace settings retrieved successfully", dto);
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceSettingsDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<WorkspaceSettingsDto>> UpdateSettingsAsync(string employerId, UpdateWorkspaceSettingsDto dto)
    {
        try
        {
            var settings = await EnsureSettingsAsync(employerId);
            settings.PrimaryContactName = dto.PrimaryContactName;
            settings.ContactEmailAddress = dto.ContactEmailAddress;
            settings.CompanyWebsite = dto.CompanyWebsite;
            settings.Industry = dto.Industry;
            settings.CompanySize = dto.CompanySize;
            settings.DefaultTeamSizeLimit = dto.DefaultTeamSizeLimit;
            settings.DefaultPtoPolicy = dto.DefaultPtoPolicy;
            settings.DefaultWorkSchedule = dto.DefaultWorkSchedule;
            settings.PrimaryTimezone = dto.PrimaryTimezone;
            settings.AutoProvisionNewHires = dto.AutoProvisionNewHires;
            settings.RequireManagerApprovalForTimeOff = dto.RequireManagerApprovalForTimeOff;
            settings.BillingEmail = dto.BillingEmail;
            settings.TaxIdOrVatNumber = dto.TaxIdOrVatNumber;
            settings.RequireTwoFactorAuthentication = dto.RequireTwoFactorAuthentication;
            settings.EnforceIpAllowlist = dto.EnforceIpAllowlist;
            settings.IdleSessionTimeout = dto.IdleSessionTimeout;
            settings.AuditLogRetention = dto.AuditLogRetention;
            settings.UpdatedAt = DateTime.UtcNow;

            await CreateSystemNotificationAsync(employerId, "Settings updated", "Organization settings were saved from the dashboard.");
            await _db.SaveChangesAsync();

            return new Response<WorkspaceSettingsDto>(
                HttpStatusCode.OK,
                "Workspace settings updated successfully",
                await BuildSettingsDtoAsync(employerId));
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceSettingsDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<WorkspaceSettingsDto>> ApplyIntegrationActionAsync(string employerId, string key, string action)
    {
        try
        {
            await EnsureSettingsAsync(employerId);
            await EnsureIntegrationsAsync(employerId);

            var integration = await _db.WorkspaceIntegrations
                .FirstAsync(x => x.EmployerId == employerId && x.Key.ToLower() == key.ToLower());

            if (action.Equals("connect", StringComparison.OrdinalIgnoreCase))
            {
                integration.IsConnected = true;
                integration.Status = "Connected";
            }
            else
            {
                integration.Status = integration.IsConnected ? "Connected" : "Not connected";
            }

            integration.UpdatedAt = DateTime.UtcNow;
            await CreateSystemNotificationAsync(employerId, $"{integration.Name} {action}", $"Workspace integration action '{action}' was applied for {integration.Name}.");
            await _db.SaveChangesAsync();

            return new Response<WorkspaceSettingsDto>(
                HttpStatusCode.OK,
                "Integration action applied successfully",
                await BuildSettingsDtoAsync(employerId));
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceSettingsDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<WorkspaceActionResultDto>> CancelPlanAsync(string employerId)
    {
        try
        {
            var settings = await EnsureSettingsAsync(employerId);
            settings.PlanName = "Free";
            settings.PlanPriceMonthly = 0;
            settings.TeamMembersLimit = 25;
            settings.ActiveProjectsLimit = 3;
            settings.UpdatedAt = DateTime.UtcNow;

            await CreateSystemNotificationAsync(employerId, "Subscription updated", "Your organization was scheduled to move to the Free plan.");
            await _db.SaveChangesAsync();

            return new Response<WorkspaceActionResultDto>(HttpStatusCode.OK, "Plan cancelled successfully", new WorkspaceActionResultDto { Message = "Plan moved to Free tier." });
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceActionResultDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<WorkspaceActionResultDto>> RequestExportAsync(string employerId)
    {
        try
        {
            await CreateSystemNotificationAsync(employerId, "Organization export requested", "A data export request was created for your workspace.");
            await _db.SaveChangesAsync();

            return new Response<WorkspaceActionResultDto>(HttpStatusCode.OK, "Export requested successfully", new WorkspaceActionResultDto { Message = "Export request accepted." });
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceActionResultDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<WorkspaceActionResultDto>> ManageSsoAsync(string employerId)
    {
        try
        {
            var settings = await EnsureSettingsAsync(employerId);
            settings.SsoConnected = true;
            settings.UpdatedAt = DateTime.UtcNow;

            await CreateSystemNotificationAsync(employerId, "SSO checked", $"{settings.SsoProviderName} SSO settings were requested from the dashboard.");
            await _db.SaveChangesAsync();

            return new Response<WorkspaceActionResultDto>(HttpStatusCode.OK, "SSO action completed successfully", new WorkspaceActionResultDto { Message = "SSO configuration opened on backend." });
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceActionResultDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<WorkspaceExportFileDto>> DownloadInvoicesAsync(string employerId)
    {
        try
        {
            var settings = await EnsureSettingsAsync(employerId);
            var projects = await _db.Projects.Where(p => p.EmployerId == employerId).ToListAsync();
            var projectIds = projects.Select(p => p.Id).ToList();
            var invoices = await _db.BudgetRecords
                .Where(r => projectIds.Contains(r.ProjectId) && r.Type == BudgetRecordType.Expense)
                .OrderByDescending(r => r.RecordDate)
                .Take(50)
                .Select(r => new { r.RecordDate, r.Description, r.Amount, r.ProjectId })
                .ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("Date,Description,Amount,ProjectId,BillingEmail,Plan");
            foreach (var invoice in invoices)
            {
                builder.AppendLine($"{invoice.RecordDate:yyyy-MM-dd},"{(invoice.Description ?? string.Empty).Replace(""", """")}",{invoice.Amount},{invoice.ProjectId},{settings.BillingEmail},{settings.PlanName}");
            }

            if (invoices.Count == 0)
            {
                builder.AppendLine($"{DateTime.UtcNow:yyyy-MM-dd},"No invoices yet",0,,{settings.BillingEmail},{settings.PlanName}");
            }

            return new Response<WorkspaceExportFileDto>(
                HttpStatusCode.OK,
                "Invoices generated successfully",
                new WorkspaceExportFileDto
                {
                    FileName = $"nexus-invoices-{DateTime.UtcNow:yyyyMMddHHmmss}.csv",
                    ContentType = "text/csv",
                    Content = builder.ToString()
                });
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceExportFileDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<WorkspaceActionResultDto>> CloseOrganizationAsync(string employerId)
    {
        try
        {
            var employer = await _db.Users.FirstOrDefaultAsync(x => x.Id == employerId && x.Role == UserRole.Employer);
            if (employer == null)
            {
                return new Response<WorkspaceActionResultDto>(HttpStatusCode.NotFound, "Employer not found");
            }

            employer.IsActive = false;
            employer.UpdatedAt = DateTime.UtcNow;

            var projects = await _db.Projects.Where(x => x.EmployerId == employerId).ToListAsync();
            foreach (var project in projects)
            {
                project.Status = ProjectStatus.Archived;
                project.UpdatedAt = DateTime.UtcNow;
            }

            await CreateSystemNotificationAsync(employerId, "Organization closed", "The organization account was marked inactive and all projects were archived.");
            await _db.SaveChangesAsync();

            return new Response<WorkspaceActionResultDto>(HttpStatusCode.OK, "Organization closed successfully", new WorkspaceActionResultDto { Message = "Organization closed and projects archived." });
        }
        catch (Exception ex)
        {
            return new Response<WorkspaceActionResultDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private async Task<WorkspaceSettingsDto> BuildSettingsDtoAsync(string employerId)
    {
        var settings = await EnsureSettingsAsync(employerId);
        var integrations = await EnsureIntegrationsAsync(employerId);
        var projects = await _db.Projects.Where(x => x.EmployerId == employerId).ToListAsync();
        var activeProjectCount = projects.Count(x => x.Status is ProjectStatus.Active or ProjectStatus.AtRisk or ProjectStatus.Planning);
        var projectIds = projects.Select(x => x.Id).ToList();
        var teamMembersUsed = await _db.ProjectMembers
            .Where(x => projectIds.Contains(x.ProjectId))
            .Select(x => x.UserId)
            .Distinct()
            .CountAsync();

        return new WorkspaceSettingsDto
        {
            Profile = new WorkspaceProfileDto
            {
                OrganizationName = settings.OrganizationName,
                OrganizationCode = settings.OrganizationCode,
                PrimaryContactName = settings.PrimaryContactName,
                ContactEmailAddress = settings.ContactEmailAddress,
                CompanyWebsite = settings.CompanyWebsite,
                Industry = settings.Industry,
                CompanySize = settings.CompanySize,
                PlanName = settings.PlanName
            },
            TeamDefaults = new WorkspaceTeamDefaultsDto
            {
                DefaultTeamSizeLimit = settings.DefaultTeamSizeLimit,
                DefaultPtoPolicy = settings.DefaultPtoPolicy,
                DefaultWorkSchedule = settings.DefaultWorkSchedule,
                PrimaryTimezone = settings.PrimaryTimezone,
                AutoProvisionNewHires = settings.AutoProvisionNewHires,
                RequireManagerApprovalForTimeOff = settings.RequireManagerApprovalForTimeOff
            },
            Billing = new WorkspaceBillingDto
            {
                PlanName = settings.PlanName,
                PlanPriceMonthly = settings.PlanPriceMonthly,
                TeamMembersUsed = teamMembersUsed,
                TeamMembersLimit = settings.TeamMembersLimit,
                ActiveProjectsUsed = activeProjectCount,
                ActiveProjectsLimit = settings.ActiveProjectsLimit,
                NextBillingDate = settings.NextBillingDate,
                PaymentMethodLast4 = settings.PaymentMethodLast4,
                BillingEmail = settings.BillingEmail,
                TaxIdOrVatNumber = settings.TaxIdOrVatNumber
            },
            Security = new WorkspaceSecurityDto
            {
                RequireTwoFactorAuthentication = settings.RequireTwoFactorAuthentication,
                EnforceIpAllowlist = settings.EnforceIpAllowlist,
                DataEncryptionAtRest = settings.DataEncryptionAtRest,
                IdleSessionTimeout = settings.IdleSessionTimeout,
                AuditLogRetention = settings.AuditLogRetention,
                SsoProviderName = settings.SsoProviderName,
                SsoConnected = settings.SsoConnected
            },
            Integrations = integrations.Select(x => new WorkspaceIntegrationDto
            {
                Key = x.Key,
                Name = x.Name,
                Status = x.Status,
                IsConnected = x.IsConnected,
                Accent = x.Accent
            }).ToList()
        };
    }

    private async Task<WorkspaceSettings> EnsureSettingsAsync(string employerId)
    {
        var settings = await _db.WorkspaceSettings.FirstOrDefaultAsync(x => x.EmployerId == employerId);
        if (settings != null)
        {
            return settings;
        }

        var employer = await _db.Users.FirstAsync(x => x.Id == employerId);
        settings = new WorkspaceSettings
        {
            Id = Guid.NewGuid(),
            EmployerId = employerId,
            OrganizationName = string.IsNullOrWhiteSpace(employer.FullName) ? "New Organization" : employer.FullName,
            OrganizationCode = $"ORG-{employerId.Replace("-", string.Empty)[..Math.Min(4, employerId.Replace("-", string.Empty).Length)].ToUpper()}",
            PrimaryContactName = employer.FullName,
            ContactEmailAddress = employer.Email,
            CompanyWebsite = string.Empty,
            Industry = "Technology",
            CompanySize = "1-50 employees",
            BillingEmail = employer.Email,
            TaxIdOrVatNumber = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.WorkspaceSettings.Add(settings);
        await _db.SaveChangesAsync();
        return settings;
    }

    private async Task<List<WorkspaceIntegration>> EnsureIntegrationsAsync(string employerId)
    {
        var integrations = await _db.WorkspaceIntegrations
            .Where(x => x.EmployerId == employerId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        if (integrations.Count > 0)
        {
            return integrations;
        }

        integrations =
        [
            new WorkspaceIntegration { Id = Guid.NewGuid(), EmployerId = employerId, Key = "okta", Name = "Okta", Status = "Connected", IsConnected = true, Accent = "okta", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new WorkspaceIntegration { Id = Guid.NewGuid(), EmployerId = employerId, Key = "google", Name = "Google Workspace", Status = "Connected", IsConnected = true, Accent = "google", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new WorkspaceIntegration { Id = Guid.NewGuid(), EmployerId = employerId, Key = "slack", Name = "Slack Enterprise", Status = "Connected", IsConnected = true, Accent = "slack", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new WorkspaceIntegration { Id = Guid.NewGuid(), EmployerId = employerId, Key = "workday", Name = "Workday", Status = "Not connected", IsConnected = false, Accent = "workday", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new WorkspaceIntegration { Id = Guid.NewGuid(), EmployerId = employerId, Key = "greenhouse", Name = "Greenhouse", Status = "Connected", IsConnected = true, Accent = "greenhouse", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new WorkspaceIntegration { Id = Guid.NewGuid(), EmployerId = employerId, Key = "salesforce", Name = "Salesforce", Status = "Not connected", IsConnected = false, Accent = "salesforce", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        ];

        _db.WorkspaceIntegrations.AddRange(integrations);
        await _db.SaveChangesAsync();
        return integrations;
    }

    private Task CreateSystemNotificationAsync(string employerId, string title, string body)
    {
        _db.EmployerNotifications.Add(new EmployerNotification
        {
            Id = Guid.NewGuid(),
            EmployerId = employerId,
            Type = EmployerNotifType.System,
            Priority = NotifPriority.Normal,
            Title = title,
            Body = body,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        return Task.CompletedTask;
    }
}