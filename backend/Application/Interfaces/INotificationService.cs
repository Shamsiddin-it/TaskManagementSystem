using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface INotificationService
{
    Task<Response<string>> AddAsync(CreateNotificationDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateNotificationDto dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<string>> MarkAsReadAsync(int id);

    Task<Response<List<GetNotificationDto>>> GetAllAsync();
    Task<Response<GetNotificationDto>> GetByIdAsync(int id);
}
