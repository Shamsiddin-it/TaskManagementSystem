[ApiController]
[Route("api/projects/{projectId:guid}/risks")]
[Authorize(Roles = "Employer")]
public class ProjectRiskController : ControllerBase
{
    private readonly IProjectRiskService _service;

    public ProjectRiskController(IProjectRiskService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> AddRisk(Guid projectId, [FromBody] CreateRiskDto dto)
    {
        var res = await _service.AddRiskAsync(projectId, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet]
    public async Task<IActionResult> GetRisks(Guid projectId)
    {
        var res = await _service.GetRisksAsync(projectId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("/api/risks/{riskId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid riskId, [FromBody] RiskStatus status)
    {
        var res = await _service.UpdateRiskStatusAsync(riskId, status);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("/api/risks/{riskId:guid}")]
    public async Task<IActionResult> DeleteRisk(Guid riskId)
    {
        var res = await _service.DeleteRiskAsync(riskId);
        return StatusCode(res.StatusCode, res);
    }
}
