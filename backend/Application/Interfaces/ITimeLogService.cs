using Application.DTOs;

namespace Application.Interfaces;

public interface ITimeLogService
{
    Task<Response<string>> AddAsync(CreateTimeLogDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateTimeLogDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<List<GetTimeLogDto>>> GetAllAsync();
    Task<Response<GetTimeLogDto>> GetByIdAsync(int id);
}
