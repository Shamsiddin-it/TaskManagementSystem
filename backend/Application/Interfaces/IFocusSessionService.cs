using Application.DTOs;

namespace Application.Interfaces;

public interface IFocusSessionService
{
    Task<Response<string>> AddAsync(CreateFocusSessionDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateFocusSessionDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<string>> StartAsync(CreateFocusSessionDto dto);
    Task<Response<string>> PauseAsync(Guid id);
    Task<Response<string>> CompleteAsync(Guid id);
    Task<Response<List<GetFocusSessionDto>>> GetAllAsync();
    Task<Response<GetFocusSessionDto>> GetByIdAsync(Guid id);
}
