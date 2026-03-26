using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class ScheduleEventController(IScheduleEventService service) : ControllerBase
{
    private readonly IScheduleEventService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateScheduleEventDto dto) => await _service.AddAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(Guid id, UpdateScheduleEventDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(Guid id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetScheduleEventDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetScheduleEventDto>> GetByIdAsync(Guid id) => await _service.GetByIdAsync(id);
}
