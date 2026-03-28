using Domain.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskEntity = Domain.Models.Task;

namespace Infrastructure.Data;

public class AuthSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly Random _random = new();

    public AuthSeeder(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async System.Threading.Tasks.Task SeedAsync()
    {
        // Keep startup seeding safe and idempotent for local development.
        if (await _dbContext.Users.AnyAsync() ||
            await _dbContext.Projects.AnyAsync() ||
            await _dbContext.Teams.AnyAsync() ||
            await _dbContext.Tasks.AnyAsync())
        {
            return;
        }

        var employer = await SeedUserAsync("Enterprise Employer", "employer@gmail.com", UserRole.Employer);

        var project = new Project
        {
            Title = "Global Nexus Alpha",
            Description = "A major multi-team project to build out Nexus 2.0",
            EmployerId = employer.Id,
            Status = ProjectStatus.Active,
            Type = ProjectType.Enterprise,
            BudgetAllocated = 500000m,
            BudgetSpent = 120500m
        };
        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync();

        for (int i = 1; i <= 3; i++)
        {
            var teamLead = await SeedUserAsync($"Team Lead {i}", $"teamlead{i}@gmail.com", UserRole.TeamLead);

            var team = new Team
            {
                ProjectId = project.Id,
                Name = $"Nexus Team {i}",
                TeamLeadId = teamLead.Id,
                Description = $"Primary engineering squad {i}."
            };
            _dbContext.Teams.Add(team);
            await _dbContext.SaveChangesAsync();

            var workerCount = _random.Next(3, 5);
            for (int w = 1; w <= workerCount; w++)
            {
                var workerEmail = $"worker{i}_{w}@gmail.com";
                var worker = await SeedUserAsync($"Worker {i}-{w}", workerEmail, UserRole.Worker);

                var member = new TeamMember
                {
                    TeamId = team.Id,
                    UserId = worker.Id,
                    DevRole = (DevRole)_random.Next(0, 5),
                    WeeklyCapacityPct = _random.Next(70, 100),
                    FocusScore = _random.Next(40, 95),
                    ThroughputPtsPerWk = _random.Next(10, 40)
                };
                _dbContext.TeamMembers.Add(member);
                await _dbContext.SaveChangesAsync();

                for (int t = 1; t <= 3; t++)
                {
                    var task = new TaskEntity
                    {
                        TicketCode = $"NEX-{i}{w}{t}",
                        Title = $"Implement feature {t} for Module {i}",
                        Description = "Highly detailed description...",
                        TeamId = team.Id,
                        AssignedToId = worker.Id,
                        CreatedById = teamLead.Id,
                        Status = (TaskStatus)_random.Next(0, 4),
                        Priority = (TaskPriority)_random.Next(0, 4),
                        TicketType = TicketType.Feature,
                        Deadline = DateTime.UtcNow.AddDays(_random.Next(-2, 14)),
                        EstimatedHours = _random.Next(2, 20),
                        StoryPoints = _random.Next(1, 13)
                    };
                    _dbContext.Tasks.Add(task);
                    await _dbContext.SaveChangesAsync();

                    if (task.Status == TaskStatus.InProgress || task.Status == TaskStatus.Done)
                    {
                        var timeLog = new TimeLog
                        {
                            TaskId = task.Id,
                            UserId = worker.Id,
                            DurationMinutes = _random.Next(30, 240),
                            StartedAt = DateTime.UtcNow.AddDays(-1).AddHours(-4),
                            EndedAt = DateTime.UtcNow.AddDays(-1),
                            IsActive = false
                        };
                        _dbContext.TimeLogs.Add(timeLog);
                    }
                }
            }
        }

        for (int u = 1; u <= 2; u++)
        {
            await SeedUserAsync($"Unassigned Worker {u}", $"unbound{u}@gmail.com", UserRole.Worker);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<ApplicationUser> SeedUserAsync(string fullName, string email, UserRole role)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return existingUser;
        }

        var names = fullName.Split(' ');
        var firstName = names[0];
        var lastName = names.Length > 1 ? names[1] : string.Empty;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, "Password123!");
        if (result.Succeeded)
        {
            var roleName = role switch
            {
                UserRole.Employer => "Employer",
                UserRole.TeamLead => "Team Lead",
                _ => "Worker"
            };
            await _userManager.AddToRoleAsync(user, roleName);
        }

        return user;
    }
}
