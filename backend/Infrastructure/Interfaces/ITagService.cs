public interface ITagService
{
    Task<Response<GetTagDto>> CreateAsync(InsertTagDto dto);
    Task<Response<GetTagDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetTagDto>>> GetAllAsync(TagFilter filter, PaginationFilter pagination);
    Task<Response<GetTagDto>> UpdateAsync(int id, UpdateTagDto dto);
    Task<Response<bool>> DeleteAsync(int id);

    Task<Response<List<GetTagDto>>> GetTeamTagsAsync(int teamId);
    Task<Response<GetTagDto>> CreateTagAsync(int teamId, InsertTagDto dto);
    Task<Response<bool>> DeleteTagAsync(int id);
    Task<Response<GetTaskTagDto>> AddTagToTaskAsync(int taskId, int tagId);
    Task<Response<bool>> RemoveTagFromTaskAsync(int taskId, int tagId);
}
