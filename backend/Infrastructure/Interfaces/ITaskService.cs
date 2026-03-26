public interface ITaskService
{
    System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> CreateAsync(Application.DTOs.CreateTaskDto dto);
    System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<Application.DTOs.GetTaskDto>>> GetAllAsync(TaskFilter filter, PaginationFilter pagination);
    System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> UpdateAsync(Guid id, Application.DTOs.UpdateTaskDto dto);
    System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> SetStatusAsync(Guid id, TaskStatus status);

    System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> CreateTaskAsync(Application.DTOs.CreateTaskDto dto);
    System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> GetTaskByIdAsync(Guid id);
    System.Threading.Tasks.Task<Response<PagedResult<Application.DTOs.GetTaskDto>>> GetTeamTasksAsync(Guid teamId, TaskQueryFilter filter);
    System.Threading.Tasks.Task<Response<PagedResult<Application.DTOs.GetTaskDto>>> GetBacklogTasksAsync(Guid teamId, PaginationFilter filter);
    System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> UpdateTaskAsync(Guid id, Application.DTOs.UpdateTaskDto dto);
    System.Threading.Tasks.Task<Response<bool>> UpdateTaskStatusAsync(Guid id, TaskStatus status);
    System.Threading.Tasks.Task<Response<bool>> AssignTaskAsync(Guid id, string newAssigneeId, string actorId);
    System.Threading.Tasks.Task<Response<bool>> ReassignTaskAsync(Guid id, string newAssigneeId, string actorId);
    System.Threading.Tasks.Task<Response<bool>> SetDeadlineAsync(Guid id, DateTime? deadline);
    System.Threading.Tasks.Task<Response<bool>> SetPriorityAsync(Guid id, TaskPriority priority);
    System.Threading.Tasks.Task<Response<bool>> MoveToSprintAsync(Guid id, Guid? sprintId);
    System.Threading.Tasks.Task<Response<bool>> DeleteTaskAsync(Guid id);
    System.Threading.Tasks.Task<Response<bool>> ReorderTasksAsync(Guid teamId, List<TaskOrderUpdateDto> updates);
    System.Threading.Tasks.Task<Response<PagedResult<Application.DTOs.GetTaskDto>>> GetBlockedTasksAsync(Guid teamId, PaginationFilter filter);
    System.Threading.Tasks.Task<Response<bool>> SetBlockedAsync(Guid id, bool isBlocked, string? reason);
    System.Threading.Tasks.Task RecalculateSprintPointsAsync(Guid sprintId);
}
