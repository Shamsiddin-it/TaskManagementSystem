using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces;

public interface ITaskCommentService
{
    Task<Response<string>> AddAsync(CreateTaskCommentDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateTaskCommentDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<string>> AddCommentAsync(CreateTaskCommentDto dto);

    Task<Response<List<GetTaskCommentDto>>> GetAllAsync();
    Task<Response<GetTaskCommentDto>> GetByIdAsync(Guid id);
}
