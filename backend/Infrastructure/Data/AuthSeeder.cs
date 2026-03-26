using Domain.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AuthSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

    public AuthSeeder(ApplicationDbContext dbContext, IPasswordHasher<ApplicationUser> passwordHasher)
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
        if (existingUser != null)
        {
            if (existingUser.Role != role)
            {
                existingUser.Role = role;
                existingUser.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            return;
        }

        var user = new ApplicationUser
        {
            FirstName = fullName,
            LastName = string.Empty,
            Email = email,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.UserName = email;
        user.PasswordHash = _passwordHasher.HashPassword(user, "123456");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
}
