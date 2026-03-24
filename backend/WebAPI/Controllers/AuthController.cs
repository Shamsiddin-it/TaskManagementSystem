using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(
        ApplicationDbContext dbContext,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<AuthResponseDto>>> Register(RegisterRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var exists = await _dbContext.Users.AnyAsync(x => x.Email == email);
        if (exists)
        {
            return BadRequest(new Response<AuthResponseDto>(HttpStatusCode.BadRequest, "User with this email already exists."));
        }

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = dto.Role.ToStorageValue(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var response = new AuthResponseDto
        {
            Role = user.Role,
            Token = _jwtTokenService.GenerateToken(user),
            ExpiresAt = _jwtTokenService.GetTokenExpirationUtc()
        };

        return Ok(new Response<AuthResponseDto>(HttpStatusCode.OK, "Registration successful.", response));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<AuthResponseDto>>> Login(LoginRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user is null || !user.IsActive)
        {
            return Unauthorized(new Response<AuthResponseDto>(HttpStatusCode.Unauthorized, "Invalid email or password."));
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new Response<AuthResponseDto>(HttpStatusCode.Unauthorized, "Invalid email or password."));
        }

        var response = new AuthResponseDto
        {
            Role = user.Role,
            Token = _jwtTokenService.GenerateToken(user),
            ExpiresAt = _jwtTokenService.GetTokenExpirationUtc()
        };

        return Ok(new Response<AuthResponseDto>(HttpStatusCode.OK, "Login successful.", response));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<Response<AuthResponseDto>>> Me()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new Response<AuthResponseDto>(HttpStatusCode.Unauthorized, "Invalid token."));
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            return NotFound(new Response<AuthResponseDto>(HttpStatusCode.NotFound, "User not found."));
        }

        var response = new AuthResponseDto
        {
            Role = user.Role,
            Token = string.Empty,
            ExpiresAt = DateTime.UtcNow
        };

        return Ok(new Response<AuthResponseDto>(HttpStatusCode.OK, "Current user loaded.", response));
    }
}
