public class TimelineService : ITimelineService
{
    private readonly AppDbContext _db;
    public TimelineService(AppDbContext db) => _db = db;

    public async Task<Response<List<TimelinePhaseDto>>> GetTimelineAsync(Guid projectId)
    {
        try
        {
            var result = await _db.ProjectTimelines
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.OrderIndex)
                .Select(t => new TimelinePhaseDto
                {
                    Id = t.Id,
                    PhaseName = t.PhaseName,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    ColorHex = t.ColorHex,
                    Status = t.Status,
                    OrderIndex = t.OrderIndex
                })
                .ToListAsync();

            return new Response<List<TimelinePhaseDto>>(
                HttpStatusCode.OK, "Timeline retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<List<TimelinePhaseDto>>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<TimelinePhaseDto>> AddPhaseAsync(Guid projectId, CreateTimelinePhaseDto dto)
    {
        try
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                return new Response<TimelinePhaseDto>(HttpStatusCode.NotFound, "Project not found");

            var phase = new ProjectTimeline
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                PhaseName = dto.PhaseName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ColorHex = dto.ColorHex,
                OrderIndex = dto.OrderIndex,
                Status = GanttPhaseStatus.NotStarted
            };

            _db.ProjectTimelines.Add(phase);
            await _db.SaveChangesAsync();

            var result = new TimelinePhaseDto
            {
                Id = phase.Id,
                PhaseName = phase.PhaseName,
                StartDate = phase.StartDate,
                EndDate = phase.EndDate,
                ColorHex = phase.ColorHex,
                Status = phase.Status,
                OrderIndex = phase.OrderIndex
            };

            return new Response<TimelinePhaseDto>(
                HttpStatusCode.Created, "Phase added successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<TimelinePhaseDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> UpdatePhaseStatusAsync(Guid phaseId, GanttPhaseStatus status)
    {
        try
        {
            var phase = await _db.ProjectTimelines.FindAsync(phaseId);
            if (phase == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Phase not found");

            phase.Status = status;
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Phase status updated successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeletePhaseAsync(Guid phaseId)
    {
        try
        {
            var phase = await _db.ProjectTimelines.FindAsync(phaseId);
            if (phase == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Phase not found");

            _db.ProjectTimelines.Remove(phase);
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Phase deleted successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
