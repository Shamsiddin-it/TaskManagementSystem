public interface ISprintRetroService
{
    Task<Response<GetSprintRetroDto>> CreateAsync(InsertSprintRetroDto dto);
    Task<Response<GetSprintRetroDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetSprintRetroDto>>> GetAllAsync(SprintRetroFilter filter, PaginationFilter pagination);
    Task<Response<GetSprintRetroDto>> UpdateAsync(int id, UpdateSprintRetroDto dto);
    Task<Response<bool>> DeleteAsync(int id);

    Task<Response<GetSprintRetroDto>> CreateRetroAsync(int sprintId, InsertSprintRetroDto dto);
    Task<Response<GetSprintRetroDto>> GetRetroAsync(int sprintId);
    Task<Response<GetSprintRetroDto>> UpdateRetroAsync(int id, UpdateSprintRetroDto dto);
    Task<Response<GetRetroActionItemDto>> CreateActionItemAsync(int retroId, InsertRetroActionItemDto dto);
    Task<Response<GetRetroActionItemDto>> UpdateActionItemAsync(int id, UpdateRetroActionItemDto dto);
    Task<Response<GetRetroActionItemDto>> ToggleActionItemAsync(int id);
    Task<Response<bool>> DeleteActionItemAsync(int id);
}
