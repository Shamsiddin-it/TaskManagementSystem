[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service) => _service = service;

    [HttpPost("register/employer")]
    public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployerDto dto)
    {
        var res = await _service.RegisterEmployerAsync(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var res = await _service.LoginAsync(dto);
        return StatusCode(res.StatusCode, res);
    }
}