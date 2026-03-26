public interface IRetroActionItemService
{
    Task<Response<GetRetroActionItemDto>> CreateAsync(InsertRetroActionItemDto dto);
    Task<Response<GetRetroActionItemDto>> GetByIdAsync(Guid id);
    Task<Response<PagedResult<GetRetroActionItemDto>>> GetAllAsync(RetroActionItemFilter filter, PaginationFilter pagination);
    Task<Response<GetRetroActionItemDto>> UpdateAsync(Guid id, UpdateRetroActionItemDto dto);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<Response<bool>> SetDoneAsync(Guid id, bool isDone);
}
