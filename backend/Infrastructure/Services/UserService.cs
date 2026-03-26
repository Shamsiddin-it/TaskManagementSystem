using System.Net;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;

    public UserService(ApplicationDbContext db) => _db = db;

    public async Task<Response<List<UserDirectoryDto>>> GetDirectoryAsync(string employerId)
    {
        try
        {
            var users = await _db.Users
                .Where(u => u.Id != employerId && u.Role != UserRole.Employer)
                .OrderBy(u => u.FirstName)
                .ToListAsync();

            var userIds = users.Select(u => u.Id).ToList();

            var projectMemberships = await _db.ProjectMembers
                .Where(m => userIds.Contains(m.UserId))
                .Join(_db.Projects, m => m.ProjectId, p => p.Id, (m, p) => new
                {
                    m.UserId,
                    ProjectTitle = p.Title
                })
                .ToListAsync();

            var skills = await _db.MemberSkills
                .Where(s => userIds.Contains(s.UserId))
                .OrderBy(s => s.OrderIndex)
                .ToListAsync();

            var result = users.Select(user =>
            {
                var currentProjects = projectMemberships
                    .Where(x => x.UserId == user.Id)
                    .Select(x => x.ProjectTitle)
                    .Distinct()
                    .ToList();

                return new UserDirectoryDto
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    OnlineStatus = user.OnlineStatus,
                    AvatarInitials = user.AvatarInitials,
                    AvatarColor = user.AvatarColor,
                    CurrentProjects = currentProjects,
                    Skills = skills
                        .Where(s => s.UserId == user.Id)
                        .Select(s => s.SkillName)
                        .ToList(),
                    WorkloadPercent = Math.Min(currentProjects.Count * 25, 100)
                };
            }).ToList();

            return new Response<List<UserDirectoryDto>>(
                HttpStatusCode.OK, "Directory retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<List<UserDirectoryDto>>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<UserDirectoryDto>> CreateUserAsync(string employerId, CreateUserDto dto)
    {
        try
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
            {
                return new Response<UserDirectoryDto>(
                    HttpStatusCode.Conflict, "Email already in use");
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                AvatarInitials = GetInitials(dto.FirstName + " " + dto.LastName),
                AvatarColor = PickAvatarColor(dto.FirstName + " " + dto.LastName),
                OnlineStatus = dto.OnlineStatus,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);

            var skills = (dto.Skills ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select((skill, index) => new MemberSkill
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    SkillName = skill.Trim(),
                    OrderIndex = index
                })
                .ToList();

            if (skills.Count > 0)
            {
                _db.MemberSkills.AddRange(skills);
            }

            _db.EmployerNotifications.Add(new EmployerNotification
            {
                Id = Guid.NewGuid(),
                EmployerId = employerId,
                Type = EmployerNotifType.TeamUpdate,
                Priority = NotifPriority.Normal,
                Title = $"{dto.FirstName} {dto.LastName} added to workspace",
                Body = $"{dto.Role} account created successfully.",
                ActionLabel = "View Detail",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            var result = new UserDirectoryDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.Role.ToString(),
                OnlineStatus = user.OnlineStatus,
                AvatarInitials = user.AvatarInitials,
                AvatarColor = user.AvatarColor,
                Skills = skills.Select(s => s.SkillName).ToList(),
                WorkloadPercent = 0
            };

            return new Response<UserDirectoryDto>(
                HttpStatusCode.Created, "User created successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<UserDirectoryDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private static string GetInitials(string fullName)
    {
        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
            : fullName.Length >= 2 ? fullName[..2].ToUpper() : fullName.ToUpper();
    }

    private static string PickAvatarColor(string input)
    {
        string[] palette = ["#65A8FF", "#4BD36D", "#D2A0FF", "#FFAD63", "#FF6B6B"];
        var index = Math.Abs(input.GetHashCode()) % palette.Length;
        return palette[index];
    }
}