using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IScheduleEventService
{
    Task<Response<string>> AddAsync(CreateScheduleEventDto dto);
    Task<Response<string>> UpdateAsync(int id, UpdateScheduleEventDto dto);
    Task<Response<string>> DeleteAsync(int id);

    Task<Response<List<GetScheduleEventDto>>> GetAllAsync();
    Task<Response<GetScheduleEventDto>> GetByIdAsync(int id);
}
