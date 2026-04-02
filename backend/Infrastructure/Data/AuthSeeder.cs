using Domain.Enums;
using Domain.Models;
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
        var employer = await SeedUserAsync("Enterprise Employer", "employer@gmail.com", UserRole.Employer);

        var projectAlpha = await EnsureProjectAsync(
            employer.Id,
            "Global Nexus Alpha",
            "A major multi-team platform initiative for the main Nexus workspace.",
            ProjectType.Enterprise,
            ProjectStatus.Active,
            500000m,
            120500m,
            DateTime.UtcNow.AddMonths(2));

        var projectOps = await EnsureProjectAsync(
            employer.Id,
            "Operations Pulse",
            "Internal automation, reporting and process quality improvements.",
            ProjectType.Internal,
            ProjectStatus.Active,
            180000m,
            64000m,
            DateTime.UtcNow.AddMonths(1));

        var projectClient = await EnsureProjectAsync(
            employer.Id,
            "Client Orbit Portal",
            "Customer-facing portal with analytics, tickets and collaboration.",
            ProjectType.Web,
            ProjectStatus.Planning,
            260000m,
            41000m,
            DateTime.UtcNow.AddMonths(3));

        await SeedTeamBundleAsync(
            projectAlpha,
            teamNumber: 1,
            teamLeadEmail: "teamlead1@gmail.com",
            teamLeadName: "Team Lead 1",
            teamName: "Nexus Team 1",
            sprintName: "Sprint Alpha",
            workerDefinitions: new[]
            {
                new WorkerSeed("worker1_1@gmail.com", "Worker 1-1", DevRole.Frontend),
                new WorkerSeed("worker1_2@gmail.com", "Worker 1-2", DevRole.Backend),
                new WorkerSeed("worker1_3@gmail.com", "Worker 1-3", DevRole.Tester),
            });

        await SeedTeamBundleAsync(
            projectOps,
            teamNumber: 2,
            teamLeadEmail: "teamlead2@gmail.com",
            teamLeadName: "Team Lead 2",
            teamName: "Ops Automation",
            sprintName: "Sprint Beta",
            workerDefinitions: new[]
            {
                new WorkerSeed("worker2_1@gmail.com", "Worker 2-1", DevRole.Fullstack),
                new WorkerSeed("worker2_2@gmail.com", "Worker 2-2", DevRole.DevOps),
                new WorkerSeed("worker2_3@gmail.com", "Worker 2-3", DevRole.Backend),
            });

        await SeedTeamBundleAsync(
            projectClient,
            teamNumber: 3,
            teamLeadEmail: "teamlead3@gmail.com",
            teamLeadName: "Team Lead 3",
            teamName: "Client Experience",
            sprintName: "Sprint Gamma",
            workerDefinitions: new[]
            {
                new WorkerSeed("worker3_1@gmail.com", "Worker 3-1", DevRole.Frontend),
                new WorkerSeed("worker3_2@gmail.com", "Worker 3-2", DevRole.Designer),
                new WorkerSeed("worker3_3@gmail.com", "Worker 3-3", DevRole.Fullstack),
            });

        await SeedUserAsync("Unassigned Worker 1", "unbound1@gmail.com", UserRole.Worker);
        await SeedUserAsync("Unassigned Worker 2", "unbound2@gmail.com", UserRole.Worker);

        await EnsureEmployerNotificationsAsync(employer.Id, projectAlpha.Id, projectOps.Id, projectClient.Id);
        await EnsureBudgetAnalyticsAsync(projectAlpha.Id, projectOps.Id, projectClient.Id);
        await EnsureProjectRisksAsync(projectAlpha.Id, projectOps.Id, projectClient.Id);
        await _dbContext.SaveChangesAsync();
    }

    private async System.Threading.Tasks.Task SeedTeamBundleAsync(
        Project project,
        int teamNumber,
        string teamLeadEmail,
        string teamLeadName,
        string teamName,
        string sprintName,
        IReadOnlyList<WorkerSeed> workerDefinitions)
    {
        var teamLead = await SeedUserAsync(teamLeadName, teamLeadEmail, UserRole.TeamLead);
        var team = await EnsureTeamAsync(project.Id, teamName, teamLead.Id, $"Primary delivery squad for {project.Title}.");
        var sprint = await EnsureSprintAsync(team.Id, sprintName, teamNumber);

        await EnsureProjectMemberAsync(project.Id, teamLead.Id, "Lead");

        var createdWorkers = new List<ApplicationUser>();
        foreach (var workerDefinition in workerDefinitions)
        {
            var worker = await SeedUserAsync(workerDefinition.FullName, workerDefinition.Email, UserRole.Worker);
            createdWorkers.Add(worker);

            await EnsureTeamMemberAsync(team.Id, worker.Id, workerDefinition.DevRole);
            await EnsureProjectMemberAsync(project.Id, worker.Id, workerDefinition.DevRole.ToString());
        }

        await EnsureTasksAsync(team, sprint, teamLead.Id, createdWorkers);
    }

    private async System.Threading.Tasks.Task<Project> EnsureProjectAsync(
        string employerId,
        string title,
        string description,
        ProjectType type,
        ProjectStatus status,
        decimal budgetAllocated,
        decimal budgetSpent,
        DateTime deadline)
    {
        var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.EmployerId == employerId && p.Title == title);
        if (project != null)
        {
            return project;
        }

        project = new Project
        {
            Title = title,
            Description = description,
            EmployerId = employerId,
            Status = status,
            Type = type,
            GlobalDeadline = deadline,
            BudgetAllocated = budgetAllocated,
            BudgetSpent = budgetSpent,
            CompletionPercent = status == ProjectStatus.Planning ? 18 : 52,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync();
        return project;
    }

    private async System.Threading.Tasks.Task<Team> EnsureTeamAsync(Guid projectId, string name, string teamLeadId, string description)
    {
        var team = await _dbContext.Teams.FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Name == name);
        if (team == null)
        {
            team = new Team
            {
                ProjectId = projectId,
                Name = name,
                TeamLeadId = teamLeadId,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.Teams.Add(team);
            await _dbContext.SaveChangesAsync();
        }
        else if (team.TeamLeadId != teamLeadId)
        {
            team.TeamLeadId = teamLeadId;
            team.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        return team;
    }

    private async System.Threading.Tasks.Task<Sprint> EnsureSprintAsync(Guid teamId, string name, int sprintNumber)
    {
        var sprint = await _dbContext.Sprints.FirstOrDefaultAsync(s => s.TeamId == teamId && s.Name == name);
        if (sprint != null)
        {
            return sprint;
        }

        sprint = new Sprint
        {
            TeamId = teamId,
            Name = name,
            Number = sprintNumber,
            Status = SprintStatus.Active,
            StartDate = DateTime.UtcNow.Date.AddDays(-7),
            EndDate = DateTime.UtcNow.Date.AddDays(7),
            CapacityPoints = 40,
            Goal = $"Ship key deliverables for team {sprintNumber}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Sprints.Add(sprint);
        await _dbContext.SaveChangesAsync();
        return sprint;
    }

    private async System.Threading.Tasks.Task EnsureTeamMemberAsync(Guid teamId, string userId, DevRole devRole)
    {
        var teamMember = await _dbContext.TeamMembers.FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);
        if (teamMember != null)
        {
            return;
        }

        _dbContext.TeamMembers.Add(new TeamMember
        {
            TeamId = teamId,
            UserId = userId,
            DevRole = devRole,
            WeeklyCapacityPct = 70 + Math.Abs(userId.GetHashCode()) % 25,
            FocusScore = 55 + Math.Abs((userId + teamId).GetHashCode()) % 35,
            ThroughputPtsPerWk = 12 + Math.Abs((teamId + userId).GetHashCode()) % 16,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    private async System.Threading.Tasks.Task EnsureProjectMemberAsync(Guid projectId, string userId, string projectRole)
    {
        var projectMember = await _dbContext.ProjectMembers.FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);
        if (projectMember != null)
        {
            return;
        }

        _dbContext.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            ProjectRole = projectRole,
            Availability = MemberAvailability.Available,
            JoinedAt = DateTime.UtcNow.AddDays(-14),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    private async System.Threading.Tasks.Task EnsureTasksAsync(Team team, Sprint sprint, string teamLeadId, IReadOnlyList<ApplicationUser> workers)
    {
        if (await _dbContext.Tasks.AnyAsync(t => t.TeamId == team.Id))
        {
            return;
        }

        var templates = new[]
        {
            new TaskTemplate("Implement dashboard widgets", TaskStatus.InProgress, TaskPriority.High, TicketType.Feature, 8, 5),
            new TaskTemplate("Fix sprint board drag issues", TaskStatus.Todo, TaskPriority.Medium, TicketType.Bug, 6, 3),
            new TaskTemplate("Prepare API analytics endpoints", TaskStatus.Review, TaskPriority.High, TicketType.Task, 10, 8),
            new TaskTemplate("Write smoke test checklist", TaskStatus.Done, TaskPriority.Low, TicketType.QA, 4, 2)
        };

        for (var index = 0; index < templates.Length; index++)
        {
            var worker = workers[index % workers.Count];
            var template = templates[index];
            var task = new TaskEntity
            {
                TicketCode = $"NEX-{team.Name[..Math.Min(3, team.Name.Length)].ToUpperInvariant()}-{index + 1:D2}",
                Title = template.Title,
                Description = $"{template.Title} for {team.Name}.",
                TeamId = team.Id,
                AssignedToId = worker.Id,
                CreatedById = teamLeadId,
                SprintId = sprint.Id,
                Status = template.Status,
                Priority = template.Priority,
                TicketType = template.TicketType,
                Deadline = DateTime.UtcNow.AddDays(3 + index),
                EstimatedHours = template.EstimatedHours,
                StoryPoints = template.StoryPoints,
                OrderIndex = index + 1,
                TotalTimeMinutes = template.Status == TaskStatus.Done ? template.EstimatedHours * 50 : template.EstimatedHours * 20,
                CreatedAt = DateTime.UtcNow.AddDays(-5 + index),
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Tasks.Add(task);

            if (template.Status is TaskStatus.InProgress or TaskStatus.Done)
            {
                _dbContext.TimeLogs.Add(new TimeLog
                {
                    TaskId = task.Id,
                    UserId = worker.Id,
                    DurationMinutes = template.EstimatedHours * 45,
                    StartedAt = DateTime.UtcNow.AddDays(-1).AddHours(-6 + index),
                    EndedAt = DateTime.UtcNow.AddDays(-1).AddHours(-4 + index),
                    IsActive = false
                });
            }
        }

        sprint.TotalPoints = templates.Sum(x => x.StoryPoints);
        sprint.CompletedPoints = templates.Where(x => x.Status == TaskStatus.Done).Sum(x => x.StoryPoints);
        sprint.UpdatedAt = DateTime.UtcNow;
    }

    private async System.Threading.Tasks.Task EnsureEmployerNotificationsAsync(string employerId, params Guid[] projectIds)
    {
        if (await _dbContext.EmployerNotifications.AnyAsync(n => n.EmployerId == employerId))
        {
            return;
        }

        var notifications = new[]
        {
            new EmployerNotification
            {
                EmployerId = employerId,
                Type = EmployerNotifType.System,
                Priority = NotifPriority.Normal,
                Title = "Workspace demo data loaded",
                Body = "Projects, teams, sprints, tasks and users are ready for testing.",
                RelatedProjectId = projectIds.FirstOrDefault(),
                ActionLabel = "View Detail",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            },
            new EmployerNotification
            {
                EmployerId = employerId,
                Type = EmployerNotifType.MilestoneReached,
                Priority = NotifPriority.Normal,
                Title = "Sprint progress updated",
                Body = "At least one seeded sprint already has completed points for analytics.",
                RelatedProjectId = projectIds.Skip(1).FirstOrDefault(),
                ActionLabel = "Open Report",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-20)
            },
            new EmployerNotification
            {
                EmployerId = employerId,
                Type = EmployerNotifType.TeamUpdate,
                Priority = NotifPriority.High,
                Title = "Team leads assigned",
                Body = "Default Team Lead accounts are attached to seeded teams.",
                RelatedProjectId = projectIds.Skip(2).FirstOrDefault(),
                ActionLabel = "Review Team",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            }
        };

        _dbContext.EmployerNotifications.AddRange(notifications);
    }

    private async System.Threading.Tasks.Task EnsureBudgetAnalyticsAsync(params Guid[] projectIds)
    {
        if (await _dbContext.BudgetRecords.AnyAsync(r => projectIds.Contains(r.ProjectId)))
        {
            return;
        }

        var records = new List<BudgetRecord>();
        foreach (var projectId in projectIds)
        {
            records.Add(new BudgetRecord
            {
                ProjectId = projectId,
                Description = "Seed allocation",
                Amount = 50000m,
                Type = BudgetRecordType.Allocation,
                RecordDate = DateTime.UtcNow.Date.AddDays(-30),
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            });

            records.Add(new BudgetRecord
            {
                ProjectId = projectId,
                Description = "Infra and delivery expenses",
                Amount = 12650m + Math.Abs(projectId.GetHashCode()) % 7000,
                Type = BudgetRecordType.Expense,
                RecordDate = DateTime.UtcNow.Date.AddDays(-10),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            });
        }

        _dbContext.BudgetRecords.AddRange(records);
    }

    private async System.Threading.Tasks.Task EnsureProjectRisksAsync(params Guid[] projectIds)
    {
        if (await _dbContext.ProjectRisks.AnyAsync(r => projectIds.Contains(r.ProjectId)))
        {
            return;
        }

        var risks = new[]
        {
            new ProjectRisk
            {
                ProjectId = projectIds[0],
                Description = "Analytics API latency may affect dashboard freshness.",
                Severity = RiskSeverity.Medium,
                Status = RiskStatus.Open,
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new ProjectRisk
            {
                ProjectId = projectIds[1],
                Description = "Automation rollout depends on one external integration.",
                Severity = RiskSeverity.High,
                Status = RiskStatus.Mitigated,
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new ProjectRisk
            {
                ProjectId = projectIds[2],
                Description = "Client portal release scope may grow before sprint close.",
                Severity = RiskSeverity.Critical,
                Status = RiskStatus.Open,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        _dbContext.ProjectRisks.AddRange(risks);
    }

    private async System.Threading.Tasks.Task<ApplicationUser> SeedUserAsync(string fullName, string email, UserRole role)
    {
        const string defaultPassword = "Password123!";
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            var (firstName, lastName) = SplitName(fullName);
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                AvatarInitials = GetInitials(firstName, lastName),
                AvatarColor = PickAvatarColor(email),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, defaultPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unable to seed user {email}: {string.Join("; ", result.Errors.Select(x => x.Description))}");
            }
        }
        else
        {
            var needsUpdate = false;
            var (firstName, lastName) = SplitName(fullName);

            if (user.Role != role)
            {
                user.Role = role;
                needsUpdate = true;
            }

            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                user.FirstName = firstName;
                needsUpdate = true;
            }

            if (string.IsNullOrWhiteSpace(user.LastName) && !string.IsNullOrWhiteSpace(lastName))
            {
                user.LastName = lastName;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            var hasDefaultPassword = await _userManager.CheckPasswordAsync(user, defaultPassword);
            if (!hasDefaultPassword)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, defaultPassword);
                if (!resetResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to reset seeded password for {email}: {string.Join("; ", resetResult.Errors.Select(x => x.Description))}");
                }
            }
        }

        var expectedRole = role switch
        {
            UserRole.Employer => "Employer",
            UserRole.TeamLead => "Team Lead",
            _ => "Worker"
        };

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(expectedRole))
        {
            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, expectedRole);
        }

        return user;
    }

    private static (string FirstName, string LastName) SplitName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = parts.FirstOrDefault() ?? fullName;
        var lastName = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : string.Empty;
        return (firstName, lastName);
    }

    private static string GetInitials(string firstName, string lastName)
    {
        var left = string.IsNullOrWhiteSpace(firstName) ? "U" : firstName.Trim()[0].ToString().ToUpperInvariant();
        var right = string.IsNullOrWhiteSpace(lastName) ? left : lastName.Trim()[0].ToString().ToUpperInvariant();
        return $"{left}{right}";
    }

    private static string PickAvatarColor(string seed)
    {
        var palette = new[] { "#8B5CF6", "#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#EC4899" };
        var index = Math.Abs(seed.ToLowerInvariant().GetHashCode()) % palette.Length;
        return palette[index];
    }

    private sealed record WorkerSeed(string Email, string FullName, DevRole DevRole);

    private sealed record TaskTemplate(
        string Title,
        TaskStatus Status,
        TaskPriority Priority,
        TicketType TicketType,
        int EstimatedHours,
        int StoryPoints);
}
