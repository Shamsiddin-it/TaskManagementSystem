using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IAbsenceService
{
    Task<Response<string>> AddAsync(CreateAbsenceDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateAbsenceDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<string>> RequestAbsenceAsync(CreateAbsenceDto dto);

    Task<Response<List<GetAbsenceDto>>> GetAllAsync();
    Task<Response<GetAbsenceDto>> GetByIdAsync(int id);
}
