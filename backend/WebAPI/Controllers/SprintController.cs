using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

[ApiController]
[Route("api/sprints")]
public class SprintController : ControllerBase
{
    private readonly ISprintService _service;

    public SprintController(ISprintService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SprintFilter filter, [FromQuery] PaginationFilter pagination)
    {
        var res = await _service.GetAllAsync(filter, pagination);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var res = await _service.GetByIdAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InsertSprintDto dto)
    {
        var res = await _service.CreateAsync(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSprintDto dto)
    {
        var res = await _service.UpdateAsync(id, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var res = await _service.DeleteAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromQuery] SprintStatus status)
    {
        var res = await _service.SetStatusAsync(id, status);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id:guid}/stats")]
    public async Task<IActionResult> GetStats(Guid id)
    {
        var res = await _service.GetSprintStatsAsync(id);
        return StatusCode(res.StatusCode, res);
    }
}
