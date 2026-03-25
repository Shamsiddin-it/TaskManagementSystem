using Application.DTOs;

namespace Application.Interfaces;

public interface IAttachmentService
{
    Task<Response<string>> AddAsync(CreateAttachmentDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateAttachmentDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<List<GetAttachmentDto>>> GetAllAsync();
    Task<Response<GetAttachmentDto>> GetByIdAsync(int id);
}
