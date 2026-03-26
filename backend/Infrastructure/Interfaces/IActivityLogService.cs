public interface IActivityLogService
{
    Task<Response<GetActivityLogDto>> CreateAsync(InsertActivityLogDto dto);
    Task<Response<GetActivityLogDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetActivityLogDto>>> GetAllAsync(ActivityLogFilter filter, PaginationFilter pagination);
    Task<Response<GetActivityLogDto>> UpdateAsync(int id, UpdateActivityLogDto dto);
    Task<Response<bool>> DeleteAsync(int id);

    Task<Response<List<GetActivityLogDto>>> GetTeamActivityAsync(int teamId, LimitOffsetFilter filter);
    Task<Response<List<GetActivityLogDto>>> GetTaskActivityAsync(int taskId, LimitOffsetFilter filter);
    Task<Response<GetActivityLogDto>> CreateActivityLogAsync(InsertActivityLogDto dto);
}
