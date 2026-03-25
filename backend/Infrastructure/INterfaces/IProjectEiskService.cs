public interface IProjectRiskService
{
    Task<Response<ProjectRiskDto>> AddRiskAsync(Guid projectId, CreateRiskDto dto);
    Task<Response<List<ProjectRiskDto>>> GetRisksAsync(Guid projectId);
    Task<Response<bool>> UpdateRiskStatusAsync(Guid riskId, RiskStatus status);
    Task<Response<bool>> DeleteRiskAsync(Guid riskId);
}