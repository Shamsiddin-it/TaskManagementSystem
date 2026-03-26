[ApiController]
[Route("api/projects/{projectId:guid}/members")]
[Authorize(Roles = "Employer")]
public class ProjectMemberController : ControllerBase
{
    private readonly IProjectMemberService _service;
    public ProjectMemberController(IProjectMemberService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> AddMember(Guid projectId, [FromBody] AddProjectMemberDto dto)
    {
        var res = await _service.AddMemberAsync(projectId, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> RemoveMember(Guid projectId, string userId)
    {
        var res = await _service.RemoveMemberAsync(projectId, userId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet]
    public async Task<IActionResult> GetMembers(Guid projectId)
    {
        var res = await _service.GetMembersAsync(projectId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{userId}/role")]
    public async Task<IActionResult> UpdateRole(Guid projectId, string userId, [FromBody] string role)
    {
        var res = await _service.UpdateMemberRoleAsync(projectId, userId, role);
        return StatusCode(res.StatusCode, res);
    }
}
