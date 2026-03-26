public interface ITagService
{
    System.Threading.Tasks.Task<Response<GetTagDto>> CreateAsync(InsertTagDto dto);
    System.Threading.Tasks.Task<Response<GetTagDto>> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<GetTagDto>>> GetAllAsync(TagFilter filter, PaginationFilter pagination);
    System.Threading.Tasks.Task<Response<GetTagDto>> UpdateAsync(Guid id, UpdateTagDto dto);
    System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id);

    System.Threading.Tasks.Task<Response<List<GetTagDto>>> GetTeamTagsAsync(Guid teamId);
    System.Threading.Tasks.Task<Response<GetTagDto>> CreateTagAsync(Guid teamId, InsertTagDto dto);
    System.Threading.Tasks.Task<Response<bool>> DeleteTagAsync(Guid id);
    System.Threading.Tasks.Task<Response<GetTaskTagDto>> AddTagToTaskAsync(Guid taskId, Guid tagId);
    System.Threading.Tasks.Task<Response<bool>> RemoveTagFromTaskAsync(Guid taskId, Guid tagId);
}
