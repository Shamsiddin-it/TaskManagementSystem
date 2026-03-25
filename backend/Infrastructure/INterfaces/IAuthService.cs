public interface IAuthService
{
    Task<Response<AuthResponseDto>> RegisterEmployerAsync(RegisterEmployerDto dto);
    Task<Response<AuthResponseDto>> LoginAsync(LoginDto dto);
}