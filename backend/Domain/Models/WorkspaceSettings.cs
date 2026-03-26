namespace Domain.Models;

public class WorkspaceSettings : BaseEntity
{
    // public Guid Id { get; set; }
    public string EmployerId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public string PrimaryContactName { get; set; } = string.Empty;
    public string ContactEmailAddress { get; set; } = string.Empty;
    public string CompanyWebsite { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string CompanySize { get; set; } = string.Empty;
    public string PlanName { get; set; } = "Enterprise";
    public decimal PlanPriceMonthly { get; set; } = 499;
    public int TeamMembersLimit { get; set; } = 500;
    public int ActiveProjectsLimit { get; set; } = 20;
    public DateTime NextBillingDate { get; set; } = DateTime.UtcNow.Date.AddMonths(1);
    public string PaymentMethodLast4 { get; set; } = "4242";
    public string BillingEmail { get; set; } = string.Empty;
    public string TaxIdOrVatNumber { get; set; } = string.Empty;
    public int DefaultTeamSizeLimit { get; set; } = 25;
    public string DefaultPtoPolicy { get; set; } = "20 days/year";
    public string DefaultWorkSchedule { get; set; } = "Mon-Fri 9AM-5PM";
    public string PrimaryTimezone { get; set; } = "America/New_York";
    public bool AutoProvisionNewHires { get; set; } = true;
    public bool RequireManagerApprovalForTimeOff { get; set; } = true;
    public bool RequireTwoFactorAuthentication { get; set; } = true;
    public bool EnforceIpAllowlist { get; set; }
    public bool DataEncryptionAtRest { get; set; } = true;
    public string IdleSessionTimeout { get; set; } = "30 Minutes";
    public string AuditLogRetention { get; set; } = "90 Days";
    public string SsoProviderName { get; set; } = "Okta";
    public bool SsoConnected { get; set; } = true;
    // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
