[ApiController]
[Route("api/workspace")]
[Authorize(Roles = "Employer")]
public class WorkspaceController : ControllerBase
{
    private readonly IWorkspaceService _service;

    public WorkspaceController(IWorkspaceService service) => _service = service;

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.GetOverviewAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.GetSettingsAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateWorkspaceSettingsDto dto)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.UpdateSettingsAsync(employerId, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("settings/actions/cancel-plan")]
    public async Task<IActionResult> CancelPlan()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.CancelPlanAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("settings/actions/request-export")]
    public async Task<IActionResult> RequestExport()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.RequestExportAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("settings/actions/manage-sso")]
    public async Task<IActionResult> ManageSso()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.ManageSsoAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("settings/export/invoices")]
    public async Task<IActionResult> DownloadInvoices()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.DownloadInvoicesAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("settings/actions/close-organization")]
    public async Task<IActionResult> CloseOrganization()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.CloseOrganizationAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("settings/integrations/{key}")]
    public async Task<IActionResult> IntegrationAction(string key, [FromBody] WorkspaceIntegrationActionDto dto)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.ApplyIntegrationActionAsync(employerId, key, dto.Action);
        return StatusCode(res.StatusCode, res);
    }
}
