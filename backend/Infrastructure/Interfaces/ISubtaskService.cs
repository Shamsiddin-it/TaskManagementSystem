public interface ISubtaskService
{
    Task<Response<GetSubtaskDto>> CreateAsync(InsertSubtaskDto dto);
    Task<Response<GetSubtaskDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetSubtaskDto>>> GetAllAsync(SubtaskFilter filter, PaginationFilter pagination);
    Task<Response<GetSubtaskDto>> UpdateAsync(int id, UpdateSubtaskDto dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<bool>> SetCompletedAsync(int id, bool isCompleted);

    Task<Response<GetSubtaskDto>> CreateSubtaskAsync(int taskId, InsertSubtaskDto dto);
    Task<Response<GetSubtaskDto>> UpdateSubtaskAsync(int id, UpdateSubtaskDto dto);
    Task<Response<GetSubtaskDto>> ToggleSubtaskAsync(int id);
    Task<Response<bool>> DeleteSubtaskAsync(int id);
    Task<Response<bool>> ReorderSubtasksAsync(int taskId, List<SubtaskOrderUpdateDto> updates);
}
