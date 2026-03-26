using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class JoinRequestController(IJoinRequestService service) : ControllerBase
{
    private readonly IJoinRequestService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateJoinRequestDto dto) => await _service.AddAsync(dto);

    [HttpPost("apply")]
    public async Task<Response<string>> ApplyToTeamAsync(CreateJoinRequestDto dto) => await _service.ApplyToTeamAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(Guid id, UpdateJoinRequestDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(Guid id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetJoinRequestDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetJoinRequestDto>> GetByIdAsync(Guid id) => await _service.GetByIdAsync(id);
}
