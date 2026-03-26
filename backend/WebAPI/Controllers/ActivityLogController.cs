using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

[ApiController]
[Route("api/activity-logs")]
public class ActivityLogController : ControllerBase
{
    private readonly IActivityLogService _service;

    public ActivityLogController(IActivityLogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ActivityLogFilter filter, [FromQuery] PaginationFilter pagination)
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
    public async Task<IActionResult> Create([FromBody] InsertActivityLogDto dto)
    {
        var res = await _service.CreateAsync(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateActivityLogDto dto)
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

    [HttpGet("team/{teamId:guid}")]
    public async Task<IActionResult> GetTeamActivity(Guid teamId, [FromQuery] LimitOffsetFilter filter)
    {
        var res = await _service.GetTeamActivityAsync(teamId, filter);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("task/{taskId:guid}")]
    public async Task<IActionResult> GetTaskActivity(Guid taskId, [FromQuery] LimitOffsetFilter filter)
    {
        var res = await _service.GetTaskActivityAsync(taskId, filter);
        return StatusCode(res.StatusCode, res);
    }
}
