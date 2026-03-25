public class WorkspaceIntegration
{
    public Guid Id { get; set; }
    public Guid EmployerId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Not connected";
    public bool IsConnected { get; set; }
    public string Accent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
