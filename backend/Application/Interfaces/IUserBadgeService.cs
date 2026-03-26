using Application.DTOs;

namespace Application.Interfaces;

public interface IUserBadgeService
{
    Task<Response<string>> AddAsync(CreateUserBadgeDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateUserBadgeDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<List<GetUserBadgeDto>>> GetAllAsync();
    Task<Response<GetUserBadgeDto>> GetByIdAsync(int id);
}
