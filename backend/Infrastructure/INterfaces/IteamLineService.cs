public interface ITimelineService
{
    Task<Response<List<TimelinePhaseDto>>> GetTimelineAsync(Guid projectId);
    Task<Response<TimelinePhaseDto>> AddPhaseAsync(Guid projectId, CreateTimelinePhaseDto dto);
    Task<Response<bool>> UpdatePhaseStatusAsync(Guid phaseId, GanttPhaseStatus status);
    Task<Response<bool>> DeletePhaseAsync(Guid phaseId);
}