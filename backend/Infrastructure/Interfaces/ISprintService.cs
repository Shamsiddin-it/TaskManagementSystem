public interface ISprintService
{
    System.Threading.Tasks.Task<Response<GetSprintDto>> CreateAsync(InsertSprintDto dto);
    System.Threading.Tasks.Task<Response<GetSprintDto>> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<GetSprintDto>>> GetAllAsync(SprintFilter filter, PaginationFilter pagination);
    System.Threading.Tasks.Task<Response<GetSprintDto>> UpdateAsync(Guid id, UpdateSprintDto dto);
    System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> SetStatusAsync(Guid id, SprintStatus status);

    System.Threading.Tasks.Task<Response<GetSprintDto>> CreateSprintAsync(Guid teamId, InsertSprintDto dto);
    System.Threading.Tasks.Task<Response<PagedResult<GetSprintDto>>> GetSprintsAsync(Guid teamId, SprintQueryFilter filter);
    System.Threading.Tasks.Task<Response<GetSprintDto>> GetActiveSprintAsync(Guid teamId);
    System.Threading.Tasks.Task<Response<GetSprintDto>> GetSprintByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> StartSprintAsync(Guid id, string actorId);
    System.Threading.Tasks.Task<Response<bool>> CompleteSprintAsync(Guid id);
    System.Threading.Tasks.Task<Response<GetSprintDto>> UpdateSprintAsync(Guid id, UpdateSprintDto dto);
    System.Threading.Tasks.Task<Response<SprintStatsDto>> GetSprintStatsAsync(Guid id);
}
