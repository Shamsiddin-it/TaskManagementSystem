using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces;

public interface IDaySummaryService
{
    Task<Response<string>> AddAsync(CreateDaySummaryDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateDaySummaryDto dto);
    Task<Response<string>> DeleteAsync(Guid id);

    Task<Response<List<GetDaySummaryDto>>> GetAllAsync();
    Task<Response<GetDaySummaryDto>> GetByIdAsync(Guid id);
}
