using System.Net;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.DTOs;
using Microsoft.Extensions.Configuration;
using Domain.Enums;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{

    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<Response<AuthResponseDto>> RegisterEmployerAsync(RegisterEmployerDto dto)
    {
        try
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return new Response<AuthResponseDto>(
                    HttpStatusCode.Conflict, "Email already in use");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = dto.FirstName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Domain.Enums.UserRole.Employer,
                AvatarInitials = GetInitials(dto.FirstName),
                IsActive = true,
                OnlineStatus = OnlineStatus.Online,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Add(user);
            await _db.SaveChangesAsync();

            var result = new AuthResponseDto
            {
                Token = GenerateJwt(user),
                Email = user.Email,
                Role = MapRole(user.Role),
                UserId = user.Id,
                FirstName = user.FirstName,
                FullName = user.FullName
            };

            return new Response<AuthResponseDto>(
                HttpStatusCode.Created, "Employer registered successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<AuthResponseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        try
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new Response<AuthResponseDto>(
                    HttpStatusCode.Unauthorized, "Invalid credentials");

            if (!user.IsActive)
                return new Response<AuthResponseDto>(
                    HttpStatusCode.Forbidden, "Account is deactivated");

            user.LastActiveAt = DateTime.UtcNow;
            user.OnlineStatus = OnlineStatus.Online;
            await _db.SaveChangesAsync();

            Guid? teamId = null;
            if (user.Role == UserRole.TeamLead)
            {
                teamId = await _db.Teams
                    .Where(t => t.TeamLeadId == user.Id)
                    .Select(t => (Guid?)t.Id)
                    .FirstOrDefaultAsync();
            }

            var result = new AuthResponseDto
            {
                Token = GenerateJwt(user),
                Email = user.Email,
                Role = MapRole(user.Role),
                UserId = user.Id,
                FirstName = user.FirstName,
                FullName = user.FullName,
                TeamId = teamId
            };

            return new Response<AuthResponseDto>(
                HttpStatusCode.OK, "Login successful", result);
        }
        catch (Exception ex)
        {
            return new Response<AuthResponseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private string GenerateJwt(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GetInitials(string fullName)
    {
        var parts = fullName.Trim().Split(' ');
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
            : fullName.Length >= 2 ? fullName[..2].ToUpper() : fullName.ToUpper();
    }

    private static string MapRole(UserRole role) => role switch
    {
        UserRole.Employer => "Employer",
        UserRole.TeamLead => "Team Lead",
        _ => "Worker"
    };
}
