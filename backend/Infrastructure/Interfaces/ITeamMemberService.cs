using Domain.Models;
public interface ITeamMemberService
{
    Task<Response<GetTeamMemberDto>> CreateAsync(InsertTeamMemberDto dto);
    Task<Response<GetTeamMemberDto>> GetByIdAsync(Guid id);
    Task<Response<PagedResult<GetTeamMemberDto>>> GetAllAsync(TeamMemberFilter filter, PaginationFilter pagination);
    Task<Response<GetTeamMemberDto>> UpdateAsync(Guid id, UpdateTeamMemberDto dto);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<Response<bool>> SetActiveAsync(Guid id, bool isActive);

    Task<Response<List<GetTeamMemberDto>>> GetTeamMembersAsync(Guid teamId);
    Task<Response<GetTeamMemberDto>> GetMemberProfileAsync(Guid teamId, string userId);
    Task<Response<bool>> AssignDevRoleAsync(Guid teamId, string userId, DevRole role, string actorId);
    Task<Response<bool>> RemoveMemberAsync(Guid teamId, string userId, string actorId);
    Task<Response<bool>> AcceptJoinRequestAsync(Guid joinRequestId);
    Task<Response<bool>> RejectJoinRequestAsync(Guid joinRequestId);
    Task<Response<List<GetJoinRequestDto>>> GetJoinRequestsAsync(Guid teamId, PaginationFilter filter);
    Task<Response<bool>> UpdateCapacityAsync(Guid teamId, string userId);
    System.Threading.Tasks.Task RecalculateCapacityAsync(string userId);
}
