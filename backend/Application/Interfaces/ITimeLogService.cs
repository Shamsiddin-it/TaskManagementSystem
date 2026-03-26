using Application.DTOs;

namespace Application.Interfaces;

public interface ITimeLogService
{
    Task<Response<string>> AddAsync(CreateTimeLogDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateTimeLogDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<List<GetTimeLogDto>>> GetAllAsync();
    Task<Response<GetTimeLogDto>> GetByIdAsync(Guid id);
}
