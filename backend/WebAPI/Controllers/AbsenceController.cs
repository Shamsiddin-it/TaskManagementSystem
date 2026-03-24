using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "WorkerOnly")]
public class AbsenceController(IAbsenceService service) : ControllerBase
{
    private readonly IAbsenceService _service = service;

    [HttpPost]
    public async Task<Response<string>> AddAsync(CreateAbsenceDto dto) => await _service.AddAsync(dto);

    [HttpPost("request")]
    public async Task<Response<string>> RequestAbsenceAsync(CreateAbsenceDto dto) => await _service.RequestAbsenceAsync(dto);

    [HttpPut("{id}")]
    public async Task<Response<string>> UpdateAsync(int id, UpdateAbsenceDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteAsync(int id) => await _service.DeleteAsync(id);

    [HttpGet]
    public async Task<Response<List<GetAbsenceDto>>> GetAllAsync() => await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<Response<GetAbsenceDto>> GetByIdAsync(int id) => await _service.GetByIdAsync(id);
}
