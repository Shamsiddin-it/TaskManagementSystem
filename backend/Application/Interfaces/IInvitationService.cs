using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IInvitationService
{
    Task<Response<string>> AddAsync(CreateInvitationDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateInvitationDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<string>> AcceptAsync(int id);
    Task<Response<string>> RejectAsync(int id);

    Task<Response<List<GetInvitationDto>>> GetAllAsync();
    Task<Response<GetInvitationDto>> GetByIdAsync(int id);
}
