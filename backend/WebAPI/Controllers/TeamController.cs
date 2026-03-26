[ApiController]
[Route("api")]
[Authorize(Roles = "Employer")]
public class TeamController : ControllerBase
{
    private readonly ITeamService _service;
    public TeamController(ITeamService service) => _service = service;

    [HttpPost("projects/{projectId:guid}/teams")]
    public async Task<IActionResult> CreateTeam(Guid projectId, [FromBody] CreateTeamDto dto)
    {
        var res = await _service.CreateTeamAsync(projectId, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("teams/{teamId:guid}")]
    public async Task<IActionResult> UpdateTeam(Guid teamId, [FromBody] UpdateTeamDto dto)
    {
        var res = await _service.UpdateTeamAsync(teamId, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("teams/{teamId:guid}/team-lead")]
    public async Task<IActionResult> AssignTeamLead(Guid teamId, [FromBody] AssignTeamLeadDto dto)
    {
        var res = await _service.AssignTeamLeadAsync(teamId, dto.TeamLeadId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("teams/{teamId:guid}/team-lead")]
    public async Task<IActionResult> RemoveTeamLead(Guid teamId)
    {
        var res = await _service.RemoveTeamLeadAsync(teamId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("teams/{teamId:guid}/progress")]
    public async Task<IActionResult> GetTeamProgress(Guid teamId)
    {
        var res = await _service.GetTeamProgressAsync(teamId);
        return StatusCode(res.StatusCode, res);
    }
}
