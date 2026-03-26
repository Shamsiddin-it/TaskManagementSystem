using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IJoinRequestService
{
    Task<Response<string>> AddAsync(CreateJoinRequestDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateJoinRequestDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<string>> ApplyToTeamAsync(CreateJoinRequestDto dto);

    Task<Response<List<GetJoinRequestDto>>> GetAllAsync();
    Task<Response<GetJoinRequestDto>> GetByIdAsync(int id);
}
