using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces;

public interface IScheduleEventService
{
    Task<Response<string>> AddAsync(CreateScheduleEventDto dto);
    Task<Response<string>> UpdateAsync(Guid id, UpdateScheduleEventDto dto);
    Task<Response<string>> DeleteAsync(Guid id);

    Task<Response<List<GetScheduleEventDto>>> GetAllAsync();
    Task<Response<GetScheduleEventDto>> GetByIdAsync(Guid id);
}
