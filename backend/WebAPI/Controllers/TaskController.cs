using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _service;

    public TaskController(ITaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskFilter filter, [FromQuery] PaginationFilter pagination)
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
    public async Task<IActionResult> Create([FromBody] InsertTaskDto dto)
    {
        var res = await _service.CreateAsync(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
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
    public async Task<IActionResult> SetStatus(Guid id, [FromQuery] TaskStatus status)
    {
        var res = await _service.SetStatusAsync(id, status);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/blocked")]
    public async Task<IActionResult> SetBlocked(Guid id, [FromQuery] bool isBlocked, [FromQuery] string? reason)
    {
        var res = await _service.SetBlockedAsync(id, isBlocked, reason);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/deadline")]
    public async Task<IActionResult> SetDeadline(Guid id, [FromQuery] DateTime? deadline)
    {
        var res = await _service.SetDeadlineAsync(id, deadline);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/priority")]
    public async Task<IActionResult> SetPriority(Guid id, [FromQuery] TaskPriority priority)
    {
        var res = await _service.SetPriorityAsync(id, priority);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromQuery] string userId, [FromQuery] string actorId)
    {
        var res = await _service.AssignTaskAsync(id, userId, actorId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/sprint")]
    public async Task<IActionResult> MoveToSprint(Guid id, [FromQuery] Guid? sprintId)
    {
        var res = await _service.MoveToSprintAsync(id, sprintId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("team/{teamId:guid}/reorder")]
    public async Task<IActionResult> Reorder(Guid teamId, [FromBody] List<TaskOrderUpdateDto> updates)
    {
        var res = await _service.ReorderTasksAsync(teamId, updates);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:guid}")]
    public async Task<IActionResult> GetTeamTasks(Guid teamId, [FromQuery] TaskQueryFilter filter)
    {
        var res = await _service.GetTeamTasksAsync(teamId, filter);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:guid}/backlog")]
    public async Task<IActionResult> GetBacklog(Guid teamId, [FromQuery] PaginationFilter pagination)
    {
        var res = await _service.GetBacklogTasksAsync(teamId, pagination);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:guid}/blocked")]
    public async Task<IActionResult> GetBlocked(Guid teamId, [FromQuery] PaginationFilter pagination)
    {
        var res = await _service.GetBlockedTasksAsync(teamId, pagination);
        return StatusCode(res.StatusCode, res);
    }
}
