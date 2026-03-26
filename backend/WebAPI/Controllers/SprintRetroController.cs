using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

[ApiController]
[Route("api/sprint-retros")]
public class SprintRetroController : ControllerBase
{
    private readonly ISprintRetroService _service;

    public SprintRetroController(ISprintRetroService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SprintRetroFilter filter, [FromQuery] PaginationFilter pagination)
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
    public async Task<IActionResult> Create([FromBody] InsertSprintRetroDto dto)
    {
        var res = await _service.CreateAsync(dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSprintRetroDto dto)
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

    [HttpGet("sprint/{sprintId:guid}")]
    public async Task<IActionResult> GetBySprint(Guid sprintId)
    {
        var res = await _service.GetRetroAsync(sprintId);
        return StatusCode(res.StatusCode, res);
    }
}
