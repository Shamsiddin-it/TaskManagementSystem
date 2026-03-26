public class WorkspaceSettingsDto
{
    public WorkspaceProfileDto Profile { get; set; } = new();
    public WorkspaceTeamDefaultsDto TeamDefaults { get; set; } = new();
    public WorkspaceBillingDto Billing { get; set; } = new();
    public WorkspaceSecurityDto Security { get; set; } = new();
    public List<WorkspaceIntegrationDto> Integrations { get; set; } = [];
}

public class WorkspaceProfileDto
{
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public string PrimaryContactName { get; set; } = string.Empty;
    public string ContactEmailAddress { get; set; } = string.Empty;
    public string CompanyWebsite { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string CompanySize { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
}

public class WorkspaceTeamDefaultsDto
{
    public int DefaultTeamSizeLimit { get; set; }
    public string DefaultPtoPolicy { get; set; } = string.Empty;
    public string DefaultWorkSchedule { get; set; } = string.Empty;
    public string PrimaryTimezone { get; set; } = string.Empty;
    public bool AutoProvisionNewHires { get; set; }
    public bool RequireManagerApprovalForTimeOff { get; set; }
}

public class WorkspaceBillingDto
{
    public string PlanName { get; set; } = string.Empty;
    public decimal PlanPriceMonthly { get; set; }
    public int TeamMembersUsed { get; set; }
    public int TeamMembersLimit { get; set; }
    public int ActiveProjectsUsed { get; set; }
    public int ActiveProjectsLimit { get; set; }
    public DateTime NextBillingDate { get; set; }
    public string PaymentMethodLast4 { get; set; } = string.Empty;
    public string BillingEmail { get; set; } = string.Empty;
    public string TaxIdOrVatNumber { get; set; } = string.Empty;
}

public class WorkspaceSecurityDto
{
    public bool RequireTwoFactorAuthentication { get; set; }
    public bool EnforceIpAllowlist { get; set; }
    public bool DataEncryptionAtRest { get; set; }
    public string IdleSessionTimeout { get; set; } = string.Empty;
    public string AuditLogRetention { get; set; } = string.Empty;
    public string SsoProviderName { get; set; } = string.Empty;
    public bool SsoConnected { get; set; }
}

public class WorkspaceIntegrationDto
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public string Accent { get; set; } = string.Empty;
}
