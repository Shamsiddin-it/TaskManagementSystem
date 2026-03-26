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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var res = await _service.UpdateAsync(id, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _service.DeleteAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id, [FromQuery] TaskStatus status)
    {
        var res = await _service.SetStatusAsync(id, status);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:int}/blocked")]
    public async Task<IActionResult> SetBlocked(int id, [FromQuery] bool isBlocked, [FromQuery] string? reason)
    {
        var res = await _service.SetBlockedAsync(id, isBlocked, reason);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:int}/deadline")]
    public async Task<IActionResult> SetDeadline(int id, [FromQuery] DateTime? deadline)
    {
        var res = await _service.SetDeadlineAsync(id, deadline);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:int}/priority")]
    public async Task<IActionResult> SetPriority(int id, [FromQuery] TaskPriority priority)
    {
        var res = await _service.SetPriorityAsync(id, priority);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> Assign(int id, [FromQuery] int userId, [FromQuery] int actorId)
    {
        var res = await _service.AssignTaskAsync(id, userId, actorId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:int}/sprint")]
    public async Task<IActionResult> MoveToSprint(int id, [FromQuery] int? sprintId)
    {
        var res = await _service.MoveToSprintAsync(id, sprintId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("team/{teamId:int}/reorder")]
    public async Task<IActionResult> Reorder(int teamId, [FromBody] List<TaskOrderUpdateDto> updates)
    {
        var res = await _service.ReorderTasksAsync(teamId, updates);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:int}")]
    public async Task<IActionResult> GetTeamTasks(int teamId, [FromQuery] TaskQueryFilter filter)
    {
        var res = await _service.GetTeamTasksAsync(teamId, filter);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:int}/backlog")]
    public async Task<IActionResult> GetBacklog(int teamId, [FromQuery] PaginationFilter pagination)
    {
        var res = await _service.GetBacklogTasksAsync(teamId, pagination);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:int}/blocked")]
    public async Task<IActionResult> GetBlocked(int teamId, [FromQuery] PaginationFilter pagination)
    {
        var res = await _service.GetBlockedTasksAsync(teamId, pagination);
        return StatusCode(res.StatusCode, res);
    }
}
