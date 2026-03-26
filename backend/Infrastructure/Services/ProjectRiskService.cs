public class ProjectRiskService : IProjectRiskService
{
    private readonly ApplicationDbContext _db;
    public ProjectRiskService(ApplicationDbContext db) => _db = db;

    public async Task<Response<ProjectRiskDto>> AddRiskAsync(Guid projectId, CreateRiskDto dto)
    {
        try
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null)
                return new Response<ProjectRiskDto>(
                    HttpStatusCode.NotFound, "Project not found");

            var risk = new ProjectRisk
            {
                ProjectId = projectId,
                Description = dto.Description,
                Severity = dto.Severity,
                Status = RiskStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            _db.ProjectRisks.Add(risk);

            if (dto.Severity is RiskSeverity.High or RiskSeverity.Critical
                && project.Status == ProjectStatus.Active)
            {
                project.Status = ProjectStatus.AtRisk;
                project.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            var result = new ProjectRiskDto
            {
                Id = risk.Id,
                Description = risk.Description,
                Severity = risk.Severity,
                Status = risk.Status,
                CreatedAt = risk.CreatedAt
            };

            return new Response<ProjectRiskDto>(
                HttpStatusCode.Created, "Risk added successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<ProjectRiskDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<List<ProjectRiskDto>>> GetRisksAsync(Guid projectId)
    {
        try
        {
            var result = await _db.ProjectRisks
                .Where(r => r.ProjectId == projectId)
                .OrderByDescending(r => r.Severity)
                .Select(r => new ProjectRiskDto
                {
                    Id = r.Id,
                    Description = r.Description,
                    Severity = r.Severity,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return new Response<List<ProjectRiskDto>>(
                HttpStatusCode.OK, "Risks retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<List<ProjectRiskDto>>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> UpdateRiskStatusAsync(Guid riskId, RiskStatus status)
    {
        try
        {
            var risk = await _db.ProjectRisks.FindAsync(riskId);
            if (risk == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Risk not found");

            risk.Status = status;
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Risk status updated successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteRiskAsync(Guid riskId)
    {
        try
        {
            var risk = await _db.ProjectRisks.FindAsync(riskId);
            if (risk == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Risk not found");

            _db.ProjectRisks.Remove(risk);
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Risk deleted successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}