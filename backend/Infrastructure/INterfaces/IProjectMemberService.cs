public interface IProjectMemberService
{
    Task<Response<ProjectMemberDto>> AddMemberAsync(Guid projectId, AddProjectMemberDto dto);
    Task<Response<bool>> RemoveMemberAsync(Guid projectId, Guid userId);
    Task<Response<List<ProjectMemberDto>>> GetMembersAsync(Guid projectId);
    Task<Response<bool>> UpdateMemberRoleAsync(Guid projectId, Guid userId, string role);
}