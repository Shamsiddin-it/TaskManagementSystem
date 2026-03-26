using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class DaySummaryController(IDaySummaryService service) : ControllerBase
{
    private readonly IDaySummaryService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateDaySummaryDto dto) => await _service.AddAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(int id, UpdateDaySummaryDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(int id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetDaySummaryDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetDaySummaryDto>> GetByIdAsync(int id) => await _service.GetByIdAsync(id);
}
