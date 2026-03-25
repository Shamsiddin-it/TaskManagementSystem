using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class AttachmentController(IAttachmentService service) : ControllerBase
{
    private readonly IAttachmentService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateAttachmentDto dto) => await _service.AddAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(int id, UpdateAttachmentDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(int id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetAttachmentDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetAttachmentDto>> GetByIdAsync(int id) => await _service.GetByIdAsync(id);
}
