public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}
