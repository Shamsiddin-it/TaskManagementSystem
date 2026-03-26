using Domain.Models;
public interface IActivityLogService
{
    System.Threading.Tasks.Task<Response<GetActivityLogDto>> CreateAsync(InsertActivityLogDto dto);
    System.Threading.Tasks.Task<Response<GetActivityLogDto>> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<GetActivityLogDto>>> GetAllAsync(ActivityLogFilter filter, PaginationFilter pagination);
    System.Threading.Tasks.Task<Response<GetActivityLogDto>> UpdateAsync(Guid id, UpdateActivityLogDto dto);
    System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id);

    System.Threading.Tasks.Task<Response<List<GetActivityLogDto>>> GetTeamActivityAsync(Guid teamId, LimitOffsetFilter filter);
    System.Threading.Tasks.Task<Response<List<GetActivityLogDto>>> GetTaskActivityAsync(Guid taskId, LimitOffsetFilter filter);
    System.Threading.Tasks.Task<Response<GetActivityLogDto>> CreateActivityLogAsync(InsertActivityLogDto dto);
}
