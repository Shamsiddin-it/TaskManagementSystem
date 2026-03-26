[ApiController]
[Route("api/budget")]
[Authorize(Roles = "Employer")]
public class BudgetController : ControllerBase
{
    private readonly IBudgetService _service;
    public BudgetController(IBudgetService service) => _service = service;

    [HttpGet("org")]
    public async Task<IActionResult> GetOrgBudget()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.GetOrgBudgetAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("org")]
    public async Task<IActionResult> UpdateOrgBudget([FromBody] UpdateOrgBudgetDto dto)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.UpdateOrgBudgetAsync(employerId, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("records")]
    public async Task<IActionResult> AddRecord([FromBody] CreateBudgetRecordDto dto)
    {
        var res = await _service.AddBudgetRecordAsync(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("projects/{projectId:guid}")]
    public async Task<IActionResult> GetProjectHistory(Guid projectId)
    {
        var res = await _service.GetProjectBudgetHistoryAsync(projectId);
        return StatusCode(res.StatusCode, res);
    }
}
