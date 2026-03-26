using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IDaySummaryService
{
    Task<Response<string>> AddAsync(CreateDaySummaryDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateDaySummaryDto dto);
    Task<Response<string>> DeleteAsync(int id);

    Task<Response<List<GetDaySummaryDto>>> GetAllAsync();
    Task<Response<GetDaySummaryDto>> GetByIdAsync(int id);
}
