using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces;

public interface INotificationService
{
    Task<Response<string>> AddAsync(CreateNotificationDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateNotificationDto dto);
    Task<Response<string>> DeleteAsync(Guid id);
    Task<Response<string>> MarkAsReadAsync(Guid id);

    Task<Response<List<GetNotificationDto>>> GetAllAsync();
    Task<Response<GetNotificationDto>> GetByIdAsync(Guid id);
}
