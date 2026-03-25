public interface IUserService
{
    Task<Response<List<UserDirectoryDto>>> GetDirectoryAsync(Guid employerId);
    Task<Response<UserDirectoryDto>> CreateUserAsync(Guid employerId, CreateUserDto dto);
}
