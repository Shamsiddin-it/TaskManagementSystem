public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _db;
    public ProjectService(ApplicationDbContext db) => _db = db;

    public async Task<Response<ProjectResponseDto>> CreateProjectAsync(string employerId, CreateProjectDto dto)
    {
        try
        {
            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                EmployerId = employerId,
                Type = dto.Type,
                Status = ProjectStatus.Planning,
                GlobalDeadline = dto.GlobalDeadline,
                BudgetAllocated = dto.BudgetAllocated,
                BudgetSpent = 0,
                CompletionPercent = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Projects.Add(project);
            _db.EmployerNotifications.Add(new EmployerNotification
            {
                EmployerId = employerId,
                Type = EmployerNotifType.System,
                Priority = NotifPriority.Normal,
                Title = $"{dto.Title} created",
                Body = "Project workspace initialized successfully.",
                ActionLabel = "View Detail",
                RelatedProjectId = project.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            var result = await BuildResponseDtoAsync(project);
            return new Response<ProjectResponseDto>(
                HttpStatusCode.Created, "Project created successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<ProjectResponseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<ProjectResponseDto>> UpdateProjectAsync(Guid projectId, UpdateProjectDto dto)
    {
        try
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null)
                return new Response<ProjectResponseDto>(
                    HttpStatusCode.NotFound, "Project not found");

            if (dto.Title != null) project.Title = dto.Title;
            if (dto.Description != null) project.Description = dto.Description;
            if (dto.Status != null) project.Status = dto.Status.Value;
            if (dto.Type != null) project.Type = dto.Type.Value;
            if (dto.GlobalDeadline != null) project.GlobalDeadline = dto.GlobalDeadline;
            if (dto.BudgetAllocated != null) project.BudgetAllocated = dto.BudgetAllocated;
            project.UpdatedAt = DateTime.UtcNow;

            _db.EmployerNotifications.Add(new EmployerNotification
            {
                EmployerId = project.EmployerId,
                Type = EmployerNotifType.TeamUpdate,
                Priority = NotifPriority.Normal,
                Title = $"{project.Title} updated",
                Body = "Project details changed successfully.",
                ActionLabel = "View Detail",
                RelatedProjectId = project.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            var result = await BuildResponseDtoAsync(project);
            return new Response<ProjectResponseDto>(
                HttpStatusCode.OK, "Project updated successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<ProjectResponseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteProjectAsync(Guid projectId)
    {
        try
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null)
                return new Response<bool>(
                    HttpStatusCode.NotFound, "Project not found");

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Project deleted successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<List<ProjectResponseDto>>> GetMyProjectsAsync(string employerId)
    {
        try
        {
            var projects = await _db.Projects
                .Where(p => p.EmployerId == employerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var result = new List<ProjectResponseDto>();
            foreach (var p in projects)
                result.Add(await BuildResponseDtoAsync(p));

            return new Response<List<ProjectResponseDto>>(
                HttpStatusCode.OK, "Projects retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<List<ProjectResponseDto>>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<ProjectResponseDto>> GetProjectByIdAsync(int projectId)
    {
        try
        {
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                return new Response<ProjectResponseDto>(
                    HttpStatusCode.NotFound, "Project not found");

            var result = await BuildResponseDtoAsync(project);
            return new Response<ProjectResponseDto>(
                HttpStatusCode.OK, "Project retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<ProjectResponseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<ProjectStatsDto>> GetProjectStatsAsync(int projectId)
    {
        try
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null)
                return new Response<ProjectStatsDto>(
                    HttpStatusCode.NotFound, "Project not found");

            var teamIds = await _db.Teams
                .Where(t => t.ProjectId == projectId)
                .Select(t => t.Id)
                .ToListAsync();

            var tasks = await _db.Tasks
                .Where(t => teamIds.Contains(t.TeamId))
                .ToListAsync();

            var risks = await _db.ProjectRisks
                .Where(r => r.ProjectId == projectId)
                .Select(r => new ProjectRiskDto
                {
                    Id = r.Id,
                    Description = r.Description,
                    Severity = r.Severity,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            var result = new ProjectStatsDto
            {
                ProjectId = projectId,
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status == TaskStatus.Done),
                BlockedTasks = tasks.Count(t => t.IsBlocked),
                TotalMembers = await _db.ProjectMembers
                    .CountAsync(m => m.ProjectId == projectId),
                CompletionPercent = tasks.Count == 0 ? 0 :
                    (int)((double)tasks.Count(t => t.Status == TaskStatus.Done)
                          / tasks.Count * 100),
                BudgetBurnPercent = project.BudgetAllocated > 0
                    ? Math.Round((project.BudgetSpent ?? 0)
                                 / project.BudgetAllocated.Value * 100, 1)
                    : 0,
                Risks = risks
            };

            return new Response<ProjectStatsDto>(
                HttpStatusCode.OK, "Stats retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<ProjectStatsDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> SetDeadlineAsync(Guid projectId, DateTime deadline)
    {
        try
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Project not found");

            project.GlobalDeadline = deadline;
            project.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Deadline updated successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> ArchiveProjectAsync(Guid projectId)
    {
        try
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Project not found");

            project.Status = ProjectStatus.Archived;
            project.UpdatedAt = DateTime.UtcNow;

            _db.EmployerNotifications.Add(new EmployerNotification
            {
                EmployerId = project.EmployerId,
                Type = EmployerNotifType.MilestoneReached,
                Priority = NotifPriority.Normal,
                Title = $"{project.Title} archived",
                Body = "Project moved to archive successfully.",
                ActionLabel = "View Detail",
                RelatedProjectId = project.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Project archived successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private async Task<ProjectResponseDto> BuildResponseDtoAsync(Project p)
    {
        var teams = await _db.Teams
            .Where(t => t.ProjectId == p.Id)
            .Select(t => new TeamSummaryDto
            {
                Id = t.Id,
                Name = t.Name,
                MemberCount = _db.TeamMembers
                    .Count(m => m.TeamId == t.Id && m.IsActive)
            })
            .ToListAsync();

        var projectMembers = await _db.ProjectMembers
            .Where(m => m.ProjectId == p.Id)
            .ToListAsync();

        var userIds = projectMembers.Select(x => x.UserId).Distinct().ToList();
        var users = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);

        var members = projectMembers
            .Select(m =>
            {
                users.TryGetValue(m.UserId, out var user);

                return new ProjectMemberDto
                {
                    UserId = m.UserId,
                    FullName = ((user?.FirstName ?? string.Empty) + " " + (user?.LastName ?? string.Empty)).Trim(),
                    AvatarInitials = user?.AvatarInitials,
                    AvatarColor = user?.AvatarColor,
                    ProjectRole = m.ProjectRole,
                    Availability = m.Availability
                };
            })
            .ToList();

        return new ProjectResponseDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            Status = p.Status,
            Type = p.Type,
            GlobalDeadline = p.GlobalDeadline,
            BudgetAllocated = p.BudgetAllocated,
            BudgetSpent = p.BudgetSpent,
            CompletionPercent = p.CompletionPercent,
            CreatedAt = p.CreatedAt,
            Teams = teams,
            Members = members
        };
    }
}