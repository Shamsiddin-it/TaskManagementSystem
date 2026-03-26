using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class InvitationController(IInvitationService service) : ControllerBase
{
    private readonly IInvitationService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateInvitationDto dto) => await _service.AddAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(Guid id, UpdateInvitationDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(Guid id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetInvitationDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetInvitationDto>> GetByIdAsync(Guid id) => await _service.GetByIdAsync(id);

    [HttpPatch("{id}/accept")]
    public async Task<Response<string>> AcceptAsync(Guid id) => await _service.AcceptAsync(id);

    [HttpPatch("{id}/reject")]
    public async Task<Response<string>> RejectAsync(Guid id) => await _service.RejectAsync(id);
}
