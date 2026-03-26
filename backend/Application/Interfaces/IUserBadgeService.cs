using Application.DTOs;

namespace Application.Interfaces;

public interface IUserBadgeService
{
    Task<Response<string>> AddAsync(CreateUserBadgeDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateUserBadgeDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<List<GetUserBadgeDto>>> GetAllAsync();
    Task<Response<GetUserBadgeDto>> GetByIdAsync(Guid id);
}
