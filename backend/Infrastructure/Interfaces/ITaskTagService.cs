public interface ITaskTagService
{
    Task<Response<GetTaskTagDto>> CreateAsync(InsertTaskTagDto dto);
    Task<Response<GetTaskTagDto>> GetByIdAsync(Guid id);
    Task<Response<PagedResult<GetTaskTagDto>>> GetAllAsync(TaskTagFilter filter, PaginationFilter pagination);
    Task<Response<GetTaskTagDto>> UpdateAsync(Guid id, UpdateTaskTagDto dto);
    Task<Response<bool>> DeleteAsync(Guid id);
}
