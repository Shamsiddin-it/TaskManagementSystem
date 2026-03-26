using Application.DTOs;

namespace Application.Interfaces;

public interface IAttachmentService
{
    Task<Response<string>> AddAsync(CreateAttachmentDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateAttachmentDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<List<GetAttachmentDto>>> GetAllAsync();
    Task<Response<GetAttachmentDto>> GetByIdAsync(Guid id);
}
