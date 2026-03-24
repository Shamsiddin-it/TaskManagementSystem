using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface ITaskCommentService
{
    Task<Response<string>> AddAsync(CreateTaskCommentDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateTaskCommentDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<string>> AddCommentAsync(CreateTaskCommentDto dto);

    Task<Response<List<GetTaskCommentDto>>> GetAllAsync();
    Task<Response<GetTaskCommentDto>> GetByIdAsync(int id);
}
