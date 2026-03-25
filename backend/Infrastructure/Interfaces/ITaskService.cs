public interface ITaskService
{
    Task<Response<GetTaskDto>> CreateAsync(InsertTaskDto dto);
    Task<Response<GetTaskDto>> GetByIdAsync(int id);
    Task<Response<PagedResult<GetTaskDto>>> GetAllAsync(TaskFilter filter, PaginationFilter pagination);
    Task<Response<GetTaskDto>> UpdateAsync(int id, UpdateTaskDto dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<bool>> SetStatusAsync(int id, TaskStatus status);

    Task<Response<GetTaskDto>> CreateTaskAsync(InsertTaskDto dto);
    Task<Response<GetTaskDto>> GetTaskByIdAsync(int id);
    Task<Response<PagedResult<GetTaskDto>>> GetTeamTasksAsync(int teamId, TaskQueryFilter filter);
    Task<Response<PagedResult<GetTaskDto>>> GetBacklogTasksAsync(int teamId, PaginationFilter filter);
    Task<Response<GetTaskDto>> UpdateTaskAsync(int id, UpdateTaskDto dto);
    Task<Response<bool>> UpdateTaskStatusAsync(int id, TaskStatus status);
    Task<Response<bool>> AssignTaskAsync(int id, int newAssigneeId, int actorId);
    Task<Response<bool>> ReassignTaskAsync(int id, int newAssigneeId, int actorId);
    Task<Response<bool>> SetDeadlineAsync(int id, DateTime? deadline);
    Task<Response<bool>> SetPriorityAsync(int id, TaskPriority priority);
    Task<Response<bool>> MoveToSprintAsync(int id, int? sprintId);
    Task<Response<bool>> DeleteTaskAsync(int id);
    Task<Response<bool>> ReorderTasksAsync(int teamId, List<TaskOrderUpdateDto> updates);
    Task<Response<PagedResult<GetTaskDto>>> GetBlockedTasksAsync(int teamId, PaginationFilter filter);
    Task<Response<bool>> SetBlockedAsync(int id, bool isBlocked, string? reason);
    System.Threading.Tasks.Task RecalculateSprintPointsAsync(int sprintId);
}
