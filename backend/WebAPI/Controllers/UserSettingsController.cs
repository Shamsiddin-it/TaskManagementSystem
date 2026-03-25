using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class UserSettingsController(IUserSettingsService service) : ControllerBase
{
    private readonly IUserSettingsService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateUserSettingsDto dto) => await _service.AddAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(int id, UpdateUserSettingsDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(int id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetUserSettingsDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetUserSettingsDto>> GetByIdAsync(int id) => await _service.GetByIdAsync(id);

    [HttpGet("user/{userId}")]
    public async Task<Response<GetUserSettingsDto>> GetByUserIdAsync(int userId) => await _service.GetByUserIdAsync(userId);
}
