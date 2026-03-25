[ApiController]
[Route("api/projects/{projectId:guid}/timeline")]
[Authorize(Roles = "Employer")]
public class TimelineController : ControllerBase
{
    private readonly ITimelineService _service;
    public TimelineController(ITimelineService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetTimeline(Guid projectId)
    {
        var res = await _service.GetTimelineAsync(projectId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost]
    public async Task<IActionResult> AddPhase(Guid projectId, [FromBody] CreateTimelinePhaseDto dto)
    {
        var res = await _service.AddPhaseAsync(projectId, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("/api/timeline/{phaseId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid phaseId, [FromBody] GanttPhaseStatus status)
    {
        var res = await _service.UpdatePhaseStatusAsync(phaseId, status);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("/api/timeline/{phaseId:guid}")]
    public async Task<IActionResult> DeletePhase(Guid phaseId)
    {
        var res = await _service.DeletePhaseAsync(phaseId);
        return StatusCode(res.StatusCode, res);
    }
}