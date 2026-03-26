public interface ISprintService
{
    Task<Response<GetSprintDto>> CreateAsync(InsertSprintDto dto);
    Task<Response<GetSprintDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetSprintDto>>> GetAllAsync(SprintFilter filter, PaginationFilter pagination);
    Task<Response<GetSprintDto>> UpdateAsync(int id, UpdateSprintDto dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<bool>> SetStatusAsync(int id, SprintStatus status);

    Task<Response<GetSprintDto>> CreateSprintAsync(int teamId, InsertSprintDto dto);
    Task<Response<PagedResult<GetSprintDto>>> GetSprintsAsync(int teamId, SprintQueryFilter filter);
    Task<Response<GetSprintDto>> GetActiveSprintAsync(int teamId);
    Task<Response<GetSprintDto>> GetSprintByIdAsync(int id);
    Task<Response<bool>> StartSprintAsync(int id, int actorId);
    Task<Response<bool>> CompleteSprintAsync(int id);
    Task<Response<GetSprintDto>> UpdateSprintAsync(int id, UpdateSprintDto dto);
    Task<Response<SprintStatsDto>> GetSprintStatsAsync(int id);
}
