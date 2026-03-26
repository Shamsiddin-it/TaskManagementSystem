public interface ITaskService
{
    System.Threading.Tasks.Task<Response<GetTaskDto>> CreateAsync(InsertTaskDto dto);
    System.Threading.Tasks.Task<Response<GetTaskDto>> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<GetTaskDto>>> GetAllAsync(TaskFilter filter, PaginationFilter pagination);
    System.Threading.Tasks.Task<Response<GetTaskDto>> UpdateAsync(Guid id, UpdateTaskDto dto);
    System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> SetStatusAsync(Guid id, TaskStatus status);

    System.Threading.Tasks.Task<Response<GetTaskDto>> CreateTaskAsync(InsertTaskDto dto);
    System.Threading.Tasks.Task<Response<GetTaskDto>> GetTaskByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<GetTaskDto>>> GetTeamTasksAsync(Guid teamId, TaskQueryFilter filter);
    System.Threading.Tasks.Task<Response<PagedResult<GetTaskDto>>> GetBacklogTasksAsync(Guid teamId, PaginationFilter filter);
    System.Threading.Tasks.Task<Response<GetTaskDto>> UpdateTaskAsync(Guid id, UpdateTaskDto dto);
    System.Threading.Tasks.Task<Response<bool>> UpdateTaskStatusAsync(Guid id, TaskStatus status);
    System.Threading.Tasks.Task<Response<bool>> AssignTaskAsync(Guid id, string newAssigneeId, string actorId);
    System.Threading.Tasks.Task<Response<bool>> ReassignTaskAsync(Guid id, string newAssigneeId, string actorId);
    System.Threading.Tasks.Task<Response<bool>> SetDeadlineAsync(Guid id, DateTime? deadline);
    System.Threading.Tasks.Task<Response<bool>> SetPriorityAsync(Guid id, TaskPriority priority);
    System.Threading.Tasks.Task<Response<bool>> MoveToSprintAsync(Guid id, Guid? sprintId);
    System.Threading.Tasks.Task<Response<bool>> DeleteTaskAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> ReorderTasksAsync(Guid teamId, List<TaskOrderUpdateDto> updates);
    System.Threading.Tasks.Task<Response<PagedResult<GetTaskDto>>> GetBlockedTasksAsync(Guid teamId, PaginationFilter filter);
    System.Threading.Tasks.Task<Response<bool>> SetBlockedAsync(Guid id, bool isBlocked, string? reason);
    System.Threading.Tasks.Task RecalculateSprintPointsAsync(Guid sprintId);
}
