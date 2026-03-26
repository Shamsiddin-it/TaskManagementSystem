public interface IProjectService
{
    Task<Response<ProjectResponseDto>> CreateProjectAsync(string employerId, CreateProjectDto dto);
    Task<Response<ProjectResponseDto>> UpdateProjectAsync(Guid projectId, UpdateProjectDto dto);
    Task<Response<bool>> DeleteProjectAsync(Guid projectId);
    Task<Response<List<ProjectResponseDto>>> GetMyProjectsAsync(string employerId);
    Task<Response<ProjectResponseDto>> GetProjectByIdAsync(Guid projectId);
    Task<Response<ProjectStatsDto>> GetProjectStatsAsync(Guid projectId);
    Task<Response<bool>> SetDeadlineAsync(Guid projectId, DateTime deadline);
    Task<Response<bool>> ArchiveProjectAsync(Guid projectId);
}