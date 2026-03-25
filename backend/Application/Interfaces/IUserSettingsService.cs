using Application.DTOs;

namespace Application.Interfaces;

public interface IUserSettingsService
{
    Task<Response<string>> AddAsync(CreateUserSettingsDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateUserSettingsDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<List<GetUserSettingsDto>>> GetAllAsync();
    Task<Response<GetUserSettingsDto>> GetByIdAsync(int id);
    Task<Response<GetUserSettingsDto>> GetByUserIdAsync(int userId);
}
