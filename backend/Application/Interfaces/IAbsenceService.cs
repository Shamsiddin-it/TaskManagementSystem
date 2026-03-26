using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces;

public interface IAbsenceService
{
    Task<Response<string>> AddAsync(CreateAbsenceDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateAbsenceDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<string>> RequestAbsenceAsync(CreateAbsenceDto dto);

    Task<Response<List<GetAbsenceDto>>> GetAllAsync();
    Task<Response<GetAbsenceDto>> GetByIdAsync(Guid id);
}
