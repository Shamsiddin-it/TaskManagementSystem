using Domain.Models;
public interface ITeamMemberService
{
    Task<Response<GetTeamMemberDto>> CreateAsync(InsertTeamMemberDto dto);
    Task<Response<GetTeamMemberDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetTeamMemberDto>>> GetAllAsync(TeamMemberFilter filter, PaginationFilter pagination);
    Task<Response<GetTeamMemberDto>> UpdateAsync(int id, UpdateTeamMemberDto dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<bool>> SetActiveAsync(int id, bool isActive);

    Task<Response<List<GetTeamMemberDto>>> GetTeamMembersAsync(int teamId);
    Task<Response<GetTeamMemberDto>> GetMemberProfileAsync(int teamId, int userId);
    Task<Response<bool>> AssignDevRoleAsync(int teamId, int userId, DevRole role, int actorId);
    Task<Response<bool>> RemoveMemberAsync(int teamId, int userId, int actorId);
    Task<Response<bool>> AcceptJoinRequestAsync(int joinRequestId);
    Task<Response<bool>> RejectJoinRequestAsync(int joinRequestId);
    Task<Response<List<GetJoinRequestDto>>> GetJoinRequestsAsync(int teamId, PaginationFilter filter);
    Task<Response<bool>> UpdateCapacityAsync(int teamId, int userId);
    System.Threading.Tasks.Task RecalculateCapacityAsync(int userId);
}
