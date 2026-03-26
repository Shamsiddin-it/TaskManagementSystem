[ApiController]
[Route("api/users")]
[Authorize(Roles = "Employer")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetDirectory()
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.GetDirectoryAsync(employerId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.CreateUserAsync(employerId, dto);
        return StatusCode(res.StatusCode, res);
    }
}
