using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class TaskCommentController(ITaskCommentService service) : ControllerBase
{
    private readonly ITaskCommentService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateTaskCommentDto dto) => await _service.AddAsync(dto);

    [HttpPost("add-comment")]
    public async Task<Response<string>> AddCommentAsync(CreateTaskCommentDto dto) => await _service.AddCommentAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(Guid id, UpdateTaskCommentDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(Guid id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetTaskCommentDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetTaskCommentDto>> GetByIdAsync(Guid id) => await _service.GetByIdAsync(id);
}
