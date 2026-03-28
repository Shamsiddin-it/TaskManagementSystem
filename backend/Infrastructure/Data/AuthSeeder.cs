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

    public AuthSeeder(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async System.Threading.Tasks.Task SeedAsync()
    {
        // 1) Ensure basic roles exist (Assuming DefaultRoles.SeedRoles handles IdentityRole creation)
        if (!await _dbContext.Users.AnyAsync(u => u.Email == "employer@gmail.com"))
        {
            // First wipe existing related data to ensure clean idempotency
            _dbContext.TimeLogs.RemoveRange(_dbContext.TimeLogs);
            _dbContext.Tasks.RemoveRange(_dbContext.Tasks);
            _dbContext.TeamMembers.RemoveRange(_dbContext.TeamMembers);
            _dbContext.Teams.RemoveRange(_dbContext.Teams);
            _dbContext.Projects.RemoveRange(_dbContext.Projects);
            await _dbContext.SaveChangesAsync();

            // Seed Employer
            var employer = await SeedUserAsync("Enterprise Employer", "employer@gmail.com", UserRole.Employer);

            // Create Master Project for Employer
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

            // 2) Seed Multiple Teams & TeamLeads
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

                // Generate 3-4 workers per team
                var workerCount = new Random().Next(3, 5);
                for (int w = 1; w <= workerCount; w++)
                {
                    var workerEmail = $"worker{i}_{w}@gmail.com";
                    var worker = await SeedUserAsync($"Worker {i}-{w}", workerEmail, UserRole.Worker);

                    // Map to Team
                    var member = new TeamMember
                    {
                        TeamId = team.Id,
                        UserId = worker.Id,
                        DevRole = (DevRole)new Random().Next(0, 5),
                        WeeklyCapacityPct = new Random().Next(70, 100),
                        FocusScore = new Random().Next(40, 95),
                        ThroughputPtsPerWk = new Random().Next(10, 40)
                    };
                    _dbContext.TeamMembers.Add(member);
                    await _dbContext.SaveChangesAsync();

                    // Generate Tasks for the worker (mix of priorities and statuses)
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
                            Status = (TaskStatus)new Random().Next(0, 4),
                            Priority = (TaskPriority)new Random().Next(0, 4),
                            TicketType = TicketType.Feature,
                            Deadline = DateTime.UtcNow.AddDays(new Random().Next(-2, 14)),
                            EstimatedHours = new Random().Next(2, 20),
                            StoryPoints = new Random().Next(1, 13)
                        };
                        _dbContext.Tasks.Add(task);
                        await _dbContext.SaveChangesAsync();

                        // Add TimeLog for progressive tasks
                        if (task.Status == TaskStatus.InProgress || task.Status == TaskStatus.Done)
                        {
                            var timeLog = new TimeLog
                            {
                                TaskId = task.Id,
                                UserId = worker.Id,
                                DurationMinutes = new Random().Next(30, 240),
                                StartedAt = DateTime.UtcNow.AddDays(-1).AddHours(-4),
                                EndedAt = DateTime.UtcNow.AddDays(-1),
                                IsActive = false
                            };
                            _dbContext.TimeLogs.Add(timeLog);
                        }
                    }
                }
            }

            // 3) Seed Unbound Workers
            for (int u = 1; u <= 2; u++)
            {
                await SeedUserAsync($"Unassigned Worker {u}", $"unbound{u}@gmail.com", UserRole.Worker);
            }

            await _dbContext.SaveChangesAsync();
        }
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
