using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class UserBadgeController(IUserBadgeService service) : ControllerBase
{
    private readonly IUserBadgeService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateUserBadgeDto dto) => await _service.AddAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(int id, UpdateUserBadgeDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(int id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetUserBadgeDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetUserBadgeDto>> GetByIdAsync(int id) => await _service.GetByIdAsync(id);
}
