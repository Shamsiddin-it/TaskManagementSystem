public interface ISprintRetroService
{
    System.Threading.Tasks.Task<Response<GetSprintRetroDto>> CreateAsync(InsertSprintRetroDto dto);
    System.Threading.Tasks.Task<Response<GetSprintRetroDto>> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<GetSprintRetroDto>>> GetAllAsync(SprintRetroFilter filter, PaginationFilter pagination);
    System.Threading.Tasks.Task<Response<GetSprintRetroDto>> UpdateAsync(Guid id, UpdateSprintRetroDto dto);
    System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id);

    System.Threading.Tasks.Task<Response<GetSprintRetroDto>> CreateRetroAsync(Guid sprintId, InsertSprintRetroDto dto);
    System.Threading.Tasks.Task<Response<GetSprintRetroDto>> GetRetroAsync(Guid sprintId);
    System.Threading.Tasks.Task<Response<GetSprintRetroDto>> UpdateRetroAsync(Guid id, UpdateSprintRetroDto dto);
    System.Threading.Tasks.Task<Response<GetRetroActionItemDto>> CreateActionItemAsync(Guid retroId, InsertRetroActionItemDto dto);
    System.Threading.Tasks.Task<Response<GetRetroActionItemDto>> UpdateActionItemAsync(Guid id, UpdateRetroActionItemDto dto);
    System.Threading.Tasks.Task<Response<GetRetroActionItemDto>> ToggleActionItemAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> DeleteActionItemAsync(Guid id);
}
