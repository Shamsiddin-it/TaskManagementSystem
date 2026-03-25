public interface ITeamService
{
    Task<Response<TeamResponseDto>> CreateTeamAsync(Guid projectId, CreateTeamDto dto);
    Task<Response<TeamResponseDto>> UpdateTeamAsync(Guid teamId, UpdateTeamDto dto);
    Task<Response<bool>> AssignTeamLeadAsync(Guid teamId, Guid teamLeadId);
    Task<Response<bool>> RemoveTeamLeadAsync(Guid teamId);
    Task<Response<TeamResponseDto>> GetTeamProgressAsync(Guid teamId);
}