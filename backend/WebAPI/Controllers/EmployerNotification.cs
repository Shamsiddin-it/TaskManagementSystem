[ApiController]
[Route("api/employer/notifications")]
[Authorize(Roles = "Employer")]
public class EmployerNotificationController : ControllerBase
{
    private readonly IEmployerNotificationService _service;
    public EmployerNotificationController(IEmployerNotificationService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var employerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var res = await _service.GetNotificationsAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var res = await _service.MarkAsReadAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var employerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var res = await _service.MarkAllAsReadAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }
}