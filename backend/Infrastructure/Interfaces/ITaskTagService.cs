public interface ITaskTagService
{
    Task<Response<GetTaskTagDto>> CreateAsync(InsertTaskTagDto dto);
    Task<Response<GetTaskTagDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetTaskTagDto>>> GetAllAsync(TaskTagFilter filter, PaginationFilter pagination);
    Task<Response<GetTaskTagDto>> UpdateAsync(int id, UpdateTaskTagDto dto);
    Task<Response<bool>> DeleteAsync(int id);
}
