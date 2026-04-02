using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task = System.Threading.Tasks.Task;

[ApiController]
[Route("api/tasks")]
[Authorize]
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
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Application.DTOs.CreateTaskDto dto)
    {
        var res = await _service.CreateAsync(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Application.DTOs.UpdateTaskDto dto)
    {
        var res = await _service.UpdateAsync(id, dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var res = await _service.DeleteAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "WorkerOwnsTask")]
    public async Task<IActionResult> SetStatus(Guid id, [FromQuery] TaskStatus status)
    {
        var res = await _service.SetStatusAsync(id, status);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/blocked")]
    [Authorize(Policy = "TaskParticipant")]
    public async Task<IActionResult> SetBlocked(Guid id, [FromQuery] bool isBlocked, [FromQuery] string? reason)
    {
        var res = await _service.SetBlockedAsync(id, isBlocked, reason);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/reject")]
    [Authorize]
    public async Task<IActionResult> Reject(Guid id, [FromQuery] string? reason)
    {
        var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _service.RejectTaskAsync(id, actorId, reason);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/deadline")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> SetDeadline(Guid id, [FromQuery] DateTime? deadline)
    {
        var res = await _service.SetDeadlineAsync(id, deadline);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/priority")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> SetPriority(Guid id, [FromQuery] TaskPriority priority)
    {
        var res = await _service.SetPriorityAsync(id, priority);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/assign")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> Assign(Guid id, [FromQuery] string userId, [FromQuery] string actorId)
    {
        var res = await _service.AssignTaskAsync(id, userId, actorId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("{id:guid}/sprint")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> MoveToSprint(Guid id, [FromQuery] Guid? sprintId)
    {
        var res = await _service.MoveToSprintAsync(id, sprintId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("team/{teamId:guid}/reorder")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> Reorder(Guid teamId, [FromBody] List<TaskOrderUpdateDto> updates)
    {
        var res = await _service.ReorderTasksAsync(teamId, updates);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:guid}")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> GetTeamTasks(Guid teamId, [FromQuery] TaskQueryFilter filter)
    {
        var res = await _service.GetTeamTasksAsync(teamId, filter);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:guid}/backlog")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> GetBacklog(Guid teamId, [FromQuery] PaginationFilter pagination)
    {
        var res = await _service.GetBacklogTasksAsync(teamId, pagination);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("team/{teamId:guid}/blocked")]
    [Authorize(Policy = "TeamLeadOnly")]
    public async Task<IActionResult> GetBlocked(Guid teamId, [FromQuery] PaginationFilter pagination)
    {
        var res = await _service.GetBlockedTasksAsync(teamId, pagination);
        return StatusCode(res.StatusCode, res);
    }
}
