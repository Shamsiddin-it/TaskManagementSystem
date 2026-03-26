using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class NotificationController(INotificationService service) : ControllerBase
{
    private readonly INotificationService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateNotificationDto dto) => await _service.AddAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(Guid id, UpdateNotificationDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(Guid id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetNotificationDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetNotificationDto>> GetByIdAsync(Guid id) => await _service.GetByIdAsync(id);

    [HttpPatch("{id}/read")]
    public async Task<Response<string>> MarkAsReadAsync(Guid id) => await _service.MarkAsReadAsync(id);
}
