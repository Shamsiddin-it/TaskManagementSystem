using Domain.Models;
public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}