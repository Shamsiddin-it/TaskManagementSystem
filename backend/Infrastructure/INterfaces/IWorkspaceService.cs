public interface IWorkspaceService
{
    Task<Response<WorkspaceOverviewDto>> GetOverviewAsync(Guid employerId);
    Task<Response<WorkspaceSettingsDto>> GetSettingsAsync(Guid employerId);
    Task<Response<WorkspaceSettingsDto>> UpdateSettingsAsync(Guid employerId, UpdateWorkspaceSettingsDto dto);
    Task<Response<WorkspaceSettingsDto>> ApplyIntegrationActionAsync(Guid employerId, string key, string action);
    Task<Response<WorkspaceActionResultDto>> CancelPlanAsync(Guid employerId);
    Task<Response<WorkspaceActionResultDto>> RequestExportAsync(Guid employerId);
    Task<Response<WorkspaceActionResultDto>> ManageSsoAsync(Guid employerId);
    Task<Response<WorkspaceExportFileDto>> DownloadInvoicesAsync(Guid employerId);
    Task<Response<WorkspaceActionResultDto>> CloseOrganizationAsync(Guid employerId);
}
