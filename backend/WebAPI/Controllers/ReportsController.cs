[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Employer")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service) => _service = service;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var employerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var res = await _service.GetDashboardAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }
}
