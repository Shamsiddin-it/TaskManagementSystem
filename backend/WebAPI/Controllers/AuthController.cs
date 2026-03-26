using System.Security.Claims;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string EmployerEmail = "employer@gmail.com";
    private const string EmployerPassword = "employer1234";
    private const string TeamLeadEmail = "u9884118@gmail.com";
    private const string TeamLeadPassword = "u12345678";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
        {
            return Conflict(new { message = "Email already in use." });
        }

        var role = NormalizeRole(dto.Email, dto.Role);

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = role,
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
            Role = user.Role.ToString(),
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
        await EnsureSpecialUsersAsync(dto.Email, dto.Password);

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

        user.Role = ResolveLoginRole(user.Email ?? dto.Email, user.Role);
        user.LastActiveAt = DateTime.UtcNow;
        user.OnlineStatus = OnlineStatus.Online;
        user.UpdatedAt = DateTime.UtcNow;

        await EnsureIdentityRoleAsync(user);
        await _userManager.UpdateAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = await _tokenService.CreateTokenAsync(user),
            Email = user.Email ?? dto.Email,
            Role = user.Role.ToString(),
            UserId = user.Id,
            FullName = user.FullName,
            FirstName = user.FirstName
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
            Role = user.Role.ToString(),
            UserId = user.Id,
            FullName = user.FullName,
            FirstName = user.FirstName
        });
    }

    private async System.Threading.Tasks.Task EnsureSpecialUsersAsync(string email, string password)
    {
        if (email.Equals(EmployerEmail, StringComparison.OrdinalIgnoreCase) && password == EmployerPassword)
        {
            await EnsureUserExists(email, password, "Employer", "Account", UserRole.Employer);
            return;
        }

        if (email.Equals(TeamLeadEmail, StringComparison.OrdinalIgnoreCase) && password == TeamLeadPassword)
        {
            await EnsureUserExists(email, password, "Team", "Lead", UserRole.TeamLead);
        }
    }

    private async System.Threading.Tasks.Task EnsureUserExists(string email, string password, string firstName, string lastName, UserRole role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            user.Role = role;
            user.FirstName = string.IsNullOrWhiteSpace(user.FirstName) ? firstName : user.FirstName;
            user.LastName = string.IsNullOrWhiteSpace(user.LastName) ? lastName : user.LastName;
            user.IsActive = true;
            user.AvatarInitials ??= GetInitials(user.FirstName, user.LastName);
            user.AvatarColor ??= GetAvatarColor(email);
            user.UpdatedAt = DateTime.UtcNow;

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, resetToken, password);
            }

            await EnsureIdentityRoleAsync(user);
            await _userManager.UpdateAsync(user);
            return;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            AvatarInitials = GetInitials(firstName, lastName),
            AvatarColor = GetAvatarColor(email),
            OnlineStatus = OnlineStatus.Offline,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", createResult.Errors.Select(static x => x.Description)));
        }

        await EnsureIdentityRoleAsync(user);
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

    private static UserRole NormalizeRole(string email, UserRole requestedRole)
    {
        if (email.Equals(EmployerEmail, StringComparison.OrdinalIgnoreCase))
        {
            return UserRole.Employer;
        }

        if (email.Equals(TeamLeadEmail, StringComparison.OrdinalIgnoreCase))
        {
            return UserRole.TeamLead;
        }

        return UserRole.Worker;
    }

    private static UserRole ResolveLoginRole(string email, UserRole currentRole)
    {
        if (email.Equals(EmployerEmail, StringComparison.OrdinalIgnoreCase))
        {
            return UserRole.Employer;
        }

        if (email.Equals(TeamLeadEmail, StringComparison.OrdinalIgnoreCase))
        {
            return UserRole.TeamLead;
        }

        return UserRole.Worker;
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
