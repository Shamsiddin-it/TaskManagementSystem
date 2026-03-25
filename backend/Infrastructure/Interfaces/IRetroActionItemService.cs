public interface IRetroActionItemService
{
    Task<Response<GetRetroActionItemDto>> CreateAsync(InsertRetroActionItemDto dto);
    Task<Response<GetRetroActionItemDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetRetroActionItemDto>>> GetAllAsync(RetroActionItemFilter filter, PaginationFilter pagination);
    Task<Response<GetRetroActionItemDto>> UpdateAsync(int id, UpdateRetroActionItemDto dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<bool>> SetDoneAsync(int id, bool isDone);
}
