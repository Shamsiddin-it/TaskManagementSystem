public interface IWorkspaceService
{
    Task<Response<WorkspaceOverviewDto>> GetOverviewAsync(string employerId);
    Task<Response<WorkspaceSettingsDto>> GetSettingsAsync(string employerId);
    Task<Response<WorkspaceSettingsDto>> UpdateSettingsAsync(string employerId, UpdateWorkspaceSettingsDto dto);
    Task<Response<WorkspaceSettingsDto>> ApplyIntegrationActionAsync(string employerId, string key, string action);
    Task<Response<WorkspaceActionResultDto>> CancelPlanAsync(string employerId);
    Task<Response<WorkspaceActionResultDto>> RequestExportAsync(string employerId);
    Task<Response<WorkspaceActionResultDto>> ManageSsoAsync(string employerId);
    Task<Response<WorkspaceExportFileDto>> DownloadInvoicesAsync(string employerId);
    Task<Response<WorkspaceActionResultDto>> CloseOrganizationAsync(string employerId);
}
