using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AuthSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthSeeder(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async System.Threading.Tasks.Task SeedAsync()
    {
        await SeedUserAsync("Employer Demo", "employer@example.com", UserRole.Employer);
        await SeedUserAsync("Team Lead Demo", "teamlead@example.com", UserRole.TeamLead);
        await SeedUserAsync("Worker Demo", "worker@example.com", UserRole.Worker);
    }

    private async System.Threading.Tasks.Task SeedUserAsync(string fullName, string email, UserRole role)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (existingUser is not null)
        {
            if (existingUser.Role != role.ToStorageValue())
            {
                existingUser.Role = role.ToStorageValue();
                existingUser.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            return;
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Role = role.ToStorageValue(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, "123456");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
}
