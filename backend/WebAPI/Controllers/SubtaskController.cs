using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

[ApiController]
[Route("api/subtasks")]
public class SubtaskController : ControllerBase
{
    private readonly ISubtaskService _service;

    public SubtaskController(ISubtaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SubtaskFilter filter, [FromQuery] PaginationFilter pagination)
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
    public async Task<IActionResult> Create([FromBody] InsertSubtaskDto dto)
    {
        var res = await _service.CreateAsync(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubtaskDto dto)
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

    [HttpPatch("{id:guid}/completed")]
    public async Task<IActionResult> SetCompleted(Guid id, [FromQuery] bool isCompleted)
    {
        var res = await _service.SetCompletedAsync(id, isCompleted);
        return StatusCode(res.StatusCode, res);
    }
}
