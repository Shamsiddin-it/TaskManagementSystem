public class UpdateWorkspaceSettingsDto
{
    public string PrimaryContactName { get; set; } = string.Empty;
    public string ContactEmailAddress { get; set; } = string.Empty;
    public string CompanyWebsite { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string CompanySize { get; set; } = string.Empty;
    public int DefaultTeamSizeLimit { get; set; }
    public string DefaultPtoPolicy { get; set; } = string.Empty;
    public string DefaultWorkSchedule { get; set; } = string.Empty;
    public string PrimaryTimezone { get; set; } = string.Empty;
    public bool AutoProvisionNewHires { get; set; }
    public bool RequireManagerApprovalForTimeOff { get; set; }
    public string BillingEmail { get; set; } = string.Empty;
    public string TaxIdOrVatNumber { get; set; } = string.Empty;
    public bool RequireTwoFactorAuthentication { get; set; }
    public bool EnforceIpAllowlist { get; set; }
    public string IdleSessionTimeout { get; set; } = string.Empty;
    public string AuditLogRetention { get; set; } = string.Empty;
}

public class WorkspaceActionResultDto
{
    public string Message { get; set; } = string.Empty;
}

public class WorkspaceExportFileDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text/csv";
    public string Content { get; set; } = string.Empty;
}

public class WorkspaceIntegrationActionDto
{
    public string Action { get; set; } = string.Empty;
}
