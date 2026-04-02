using System.Security.Claims;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _tokenService;
    private readonly ApplicationDbContext _db;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService tokenService,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _db = db;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
        {
            return Conflict(new { message = "Email already in use." });
        }
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = dto.Role,
            AvatarInitials = GetInitials(dto.FirstName, dto.LastName),
            AvatarColor = GetAvatarColor(dto.Email),
            OnlineStatus = OnlineStatus.Offline,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await EnsureIdentityRoleAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = await _tokenService.CreateTokenAsync(user),
            Email = user.Email ?? dto.Email,
            Role = GetIdentityRoleName(user.Role),
            UserId = user.Id,
            FullName = user.FullName,
            FirstName = user.FirstName
        });
    }

    [HttpPost("register/employer")]
    [AllowAnonymous]
    public Task<IActionResult> RegisterEmployer(RegisterEmployerDto dto)
    {
        var registerDto = new RegisterDto
        {
            Email = dto.Email,
            Password = dto.Password,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = UserRole.Employer
        };

        return Register(registerDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (!user.IsActive)
        {
            return Forbid();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        user.LastActiveAt = DateTime.UtcNow;
        user.OnlineStatus = OnlineStatus.Online;
        user.UpdatedAt = DateTime.UtcNow;

        await EnsureIdentityRoleAsync(user);
        await _userManager.UpdateAsync(user);

        // If team lead, find their team id
        Guid? teamId = null;
        if (user.Role == UserRole.TeamLead)
        {
            var team = await _db.Teams
                .Where(t => t.TeamLeadId == user.Id)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();
            if (team != default) teamId = team;
        }

        return Ok(new AuthResponseDto
        {
            Token = await _tokenService.CreateTokenAsync(user),
            Email = user.Email ?? dto.Email,
            Role = GetIdentityRoleName(user.Role),
            UserId = user.Id,
            FullName = user.FullName,
            FirstName = user.FirstName,
            TeamId = teamId
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { message = "Invalid token." });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(new AuthResponseDto
        {
            Token = string.Empty,
            Email = user.Email ?? string.Empty,
            Role = GetIdentityRoleName(user.Role),
            UserId = user.Id,
            FullName = user.FullName,
            FirstName = user.FirstName
        });
    }

    private async System.Threading.Tasks.Task EnsureIdentityRoleAsync(ApplicationUser user)
    {
        var expectedRole = GetIdentityRoleName(user.Role);
        var existingRoles = await _userManager.GetRolesAsync(user);

        if (!existingRoles.Contains(expectedRole))
        {
            if (existingRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, existingRoles);
            }

            await _userManager.AddToRoleAsync(user, expectedRole);
        }
    }

    private static string GetIdentityRoleName(UserRole role) => role switch
    {
        UserRole.Employer => "Employer",
        UserRole.TeamLead => "Team Lead",
        _ => "Worker"
    };

    private static string GetInitials(string firstName, string lastName)
    {
        var left = string.IsNullOrWhiteSpace(firstName) ? "U" : firstName.Trim()[0].ToString().ToUpperInvariant();
        var right = string.IsNullOrWhiteSpace(lastName) ? left : lastName.Trim()[0].ToString().ToUpperInvariant();
        return $"{left}{right}";
    }

    private static string GetAvatarColor(string seed)
    {
        var palette = new[] { "#8B5CF6", "#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#EC4899" };
        var index = Math.Abs(seed.ToLowerInvariant().GetHashCode()) % palette.Length;
        return palette[index];
    }
}
