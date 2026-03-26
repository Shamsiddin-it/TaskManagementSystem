using Application.DTOs;

namespace Application.Interfaces;

public interface IUserSettingsService
{
    Task<Response<string>> AddAsync(CreateUserSettingsDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateUserSettingsDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<List<GetUserSettingsDto>>> GetAllAsync();
    Task<Response<GetUserSettingsDto>> GetByIdAsync(Guid id);
    Task<Response<GetUserSettingsDto>> GetByUserIdAsync(string userId);
}
