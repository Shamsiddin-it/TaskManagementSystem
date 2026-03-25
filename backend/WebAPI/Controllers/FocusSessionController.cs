using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class FocusSessionController(IFocusSessionService service) : ControllerBase
{
    private readonly IFocusSessionService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateFocusSessionDto dto) => await _service.AddAsync(dto);

    [HttpPost("start")]
    public async Task<Response<string>> StartAsync(CreateFocusSessionDto dto) => await _service.StartAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(int id, UpdateFocusSessionDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(int id) => await _service.DeleteAsync(id);

    [HttpPatch("{id}/pause")]
    public async Task<Response<string>> PauseAsync(int id) => await _service.PauseAsync(id);

    [HttpPatch("{id}/complete")]
    public async Task<Response<string>> CompleteAsync(int id) => await _service.CompleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetFocusSessionDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetFocusSessionDto>> GetByIdAsync(int id) => await _service.GetByIdAsync(id);
}
