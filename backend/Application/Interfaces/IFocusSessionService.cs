using Application.DTOs;

namespace Application.Interfaces;

public interface IFocusSessionService
{
    Task<Response<string>> AddAsync(CreateFocusSessionDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateFocusSessionDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<string>> StartAsync(CreateFocusSessionDto dto);
    Task<Response<string>> PauseAsync(int id);
    Task<Response<string>> CompleteAsync(int id);
    Task<Response<List<GetFocusSessionDto>>> GetAllAsync();
    Task<Response<GetFocusSessionDto>> GetByIdAsync(int id);
}
