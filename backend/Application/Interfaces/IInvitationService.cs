using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces;

public interface IInvitationService
{
    Task<Response<string>> AddAsync(CreateInvitationDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateInvitationDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<string>> AcceptAsync(Guid id);
    Task<Response<string>> RejectAsync(Guid id);

    Task<Response<List<GetInvitationDto>>> GetAllAsync();
    Task<Response<GetInvitationDto>> GetByIdAsync(Guid id);
}
