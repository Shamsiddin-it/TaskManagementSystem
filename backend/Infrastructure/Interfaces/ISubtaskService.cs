public interface ISubtaskService
{
    System.Threading.Tasks.Task<Response<GetSubtaskDto>> CreateAsync(InsertSubtaskDto dto);
    System.Threading.Tasks.Task<Response<GetSubtaskDto>> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<GetSubtaskDto>>> GetAllAsync(SubtaskFilter filter, PaginationFilter pagination);
    System.Threading.Tasks.Task<Response<GetSubtaskDto>> UpdateAsync(Guid id, UpdateSubtaskDto dto);
    System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> SetCompletedAsync(Guid id, bool isCompleted);

    System.Threading.Tasks.Task<Response<GetSubtaskDto>> CreateSubtaskAsync(Guid taskId, InsertSubtaskDto dto);
    System.Threading.Tasks.Task<Response<GetSubtaskDto>> UpdateSubtaskAsync(Guid id, UpdateSubtaskDto dto);
    System.Threading.Tasks.Task<Response<GetSubtaskDto>> ToggleSubtaskAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> DeleteSubtaskAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> ReorderSubtasksAsync(Guid taskId, List<SubtaskOrderUpdateDto> updates);
}
