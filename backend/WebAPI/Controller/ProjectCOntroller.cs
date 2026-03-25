[ApiController]
[Route("api/projects")]
[Authorize(Roles = "Employer")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;
    public ProjectController(IProjectService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        var employerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var res = await _service.CreateProjectAsync(employerId, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProjects()
    {
        var employerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var res = await _service.GetMyProjectsAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var res = await _service.GetProjectByIdAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto dto)
    {
        var res = await _service.UpdateProjectAsync(id, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var res = await _service.DeleteProjectAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id:guid}/stats")]
    public async Task<IActionResult> GetStats(Guid id)
    {
        var res = await _service.GetProjectStatsAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/deadline")]
    public async Task<IActionResult> SetDeadline(Guid id, [FromBody] DateTime deadline)
    {
        var res = await _service.SetDeadlineAsync(id, deadline);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/archive")]
    public async Task<IActionResult> ArchiveProject(Guid id)
    {
        var res = await _service.ArchiveProjectAsync(id);
        return StatusCode(res.StatusCode, res);
    }
}