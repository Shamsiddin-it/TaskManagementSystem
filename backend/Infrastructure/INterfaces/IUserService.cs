public interface IUserService
{
    Task<Response<List<UserDirectoryDto>>> GetDirectoryAsync(string employerId);
    Task<Response<UserDirectoryDto>> CreateUserAsync(string employerId, CreateUserDto dto);
}
