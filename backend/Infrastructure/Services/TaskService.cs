using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Net;
using System.Threading.Tasks;
using Domain.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskEntity = Domain.Models.Task;

public class TaskService : ITaskService
{
    private const string EntityName = "Task";
    private const int MaxActiveTasks = 10;
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TaskService> _logger;
    private readonly ITicketCodeGenerator _ticketCodeGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TaskService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<TaskService> logger, ITicketCodeGenerator ticketCodeGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
        _ticketCodeGenerator = ticketCodeGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> CreateAsync(Application.DTOs.CreateTaskDto dto)
    {
        try
        {
            if (dto == null) return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.BadRequest, "dto is null");

            var user = _httpContextAccessor.HttpContext?.User;
            var currentUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = user?.FindFirstValue(ClaimTypes.Role);

            var assignedToId = dto.AssignedTo ?? dto.AssignedToId ?? currentUserId;
            var createdById = dto.CreatedById ?? dto.CreatedBy ?? currentUserId;

            if (string.IsNullOrWhiteSpace(createdById))
                return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.BadRequest, "createdBy is required");

            if (string.IsNullOrWhiteSpace(assignedToId))
                assignedToId = createdById;

            if (role == "Worker" && currentUserId != assignedToId)
                return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.Forbidden, "Worker can create tasks only for themselves.");

            var teamId = dto.TeamId;
            if (teamId == Guid.Empty && !string.IsNullOrWhiteSpace(assignedToId))
            {
                teamId = await _db.TeamMembers
                    .Where(x => x.UserId == assignedToId && x.IsActive)
                    .Select(x => x.TeamId)
                    .FirstOrDefaultAsync();
            }

            if (teamId == Guid.Empty)
                return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.BadRequest, "teamId is required");

            var team = await _db.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.NotFound, "team not found");

            if (role == "Team Lead" && team.TeamLeadId != currentUserId)
                return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.Forbidden, "You do not have access to this team's tasks.");

            var nextOrderIndex = await _db.Tasks
                .Where(x => x.TeamId == teamId && !x.IsArchived)
                .Select(x => (int?)x.OrderIndex)
                .MaxAsync() ?? 0;

            dto.TeamId = teamId;
            dto.AssignedTo = assignedToId;
            dto.AssignedToId = assignedToId;
            dto.CreatedBy = createdById;
            dto.CreatedById = createdById;
            dto.OrderIndex = dto.OrderIndex > 0 ? dto.OrderIndex : nextOrderIndex + 1;

            var entity = _mapper.Map<TaskEntity>(dto);
            entity.TicketCode = await _ticketCodeGenerator.GenerateTicketCodeAsync(dto.TeamId, TicketType.Task);
            entity.Status = TaskStatus.Todo;
            entity.IsBlocked = false;
            entity.IsArchived = false;
            entity.TicketType = string.IsNullOrWhiteSpace(dto.TicketType) || !Enum.TryParse<TicketType>(dto.TicketType, true, out var ticketType)
                ? TicketType.Task
                : ticketType;
            entity.SprintId = dto.SprintId;
            entity.StoryPoints = dto.StoryPoints;

            _db.Tasks.Add(entity);
            await _db.SaveChangesAsync();

            if (entity.SprintId.HasValue)
            {
                await RecalculateSprintPointsAsync(entity.SprintId.Value);
            }

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<Application.DTOs.GetTaskDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out Application.DTOs.GetTaskDto cached))
            {
                return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<Application.DTOs.GetTaskDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<PagedResult<Application.DTOs.GetTaskDto>>> GetAllAsync(TaskFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new TaskFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<Application.DTOs.GetTaskDto> cached))
            {
                return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TaskEntity> query = _db.Tasks.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Title)) query = query.Where(x => x.Title.Contains(filter.Title));
            if (filter.Status.HasValue) query = query.Where(x => x.Status == filter.Status.Value);
            if (filter.AssignedToUserId != null) query = query.Where(x => x.AssignedToId == filter.AssignedToUserId);
            if (filter.Priority.HasValue) query = query.Where(x => x.Priority == filter.Priority.Value);
            if (filter.IsBlocked.HasValue) query = query.Where(x => x.IsBlocked == filter.IsBlocked.Value);
            if (filter.IsArchived.HasValue) query = query.Where(x => x.IsArchived == filter.IsArchived.Value);
            if (filter.DeadlineFrom.HasValue) query = query.Where(x => x.Deadline >= filter.DeadlineFrom.Value);
            if (filter.DeadlineTo.HasValue) query = query.Where(x => x.Deadline <= filter.DeadlineTo.Value);
            if (filter.SprintId.HasValue) query = query.Where(x => x.SprintId == filter.SprintId.Value);

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = user?.FindFirstValue(ClaimTypes.Role);

            if (role == "Worker")
            {
                query = query.Where(x => x.AssignedToId == userId);
            }
            else if (role == "Team Lead")
            {
                var myTeamIds = await _db.Teams.Where(t => t.TeamLeadId == userId).Select(t => t.Id).ToListAsync();
                query = query.Where(x => myTeamIds.Contains(x.TeamId));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OrderIndex)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<Application.DTOs.GetTaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<Application.DTOs.GetTaskDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> UpdateAsync(Guid id, Application.DTOs.UpdateTaskDto dto)
    {
        try
        {
            if (dto == null) return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.Tasks.Include(t => t.Team).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.NotFound, "not found");

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = user?.FindFirstValue(ClaimTypes.Role);

            if (role == "Team Lead" && entity.Team.TeamLeadId != userId)
                return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.Forbidden, "You do not have access to this team's tasks.");
            if (role == "Worker" && entity.AssignedToId != userId)
                return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.Forbidden, "You are not assigned to this task.");

            if (dto.Title != null) entity.Title = dto.Title;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.AssignedTo != null) entity.AssignedToId = dto.AssignedTo;
            if (dto.Status != null && Enum.TryParse<TaskStatus>(dto.Status, true, out var status)) entity.Status = status;
            if (dto.Priority != null && Enum.TryParse<TaskPriority>(dto.Priority, true, out var priority)) entity.Priority = priority;
            if (dto.Deadline.HasValue) entity.Deadline = dto.Deadline;
            if (dto.ScheduledStart.HasValue) entity.ScheduledStart = dto.ScheduledStart;
            if (dto.ScheduledEnd.HasValue) entity.ScheduledEnd = dto.ScheduledEnd;
            if (dto.EstimatedHours.HasValue) entity.EstimatedHours = dto.EstimatedHours;
            if (dto.OrderIndex.HasValue) entity.OrderIndex = dto.OrderIndex.Value;

            await _db.SaveChangesAsync();

            if (entity.SprintId.HasValue)
            {
                await RecalculateSprintPointsAsync(entity.SprintId.Value);
            }

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<Application.DTOs.GetTaskDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<Application.DTOs.GetTaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.IsArchived = true;
            await _db.SaveChangesAsync();

            if (entity.SprintId.HasValue)
            {
                await RecalculateSprintPointsAsync(entity.SprintId.Value);
            }

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            CacheKeyStore.RemoveKey(_cache, EntityName, CacheKeyHelper.BuildIdKey(EntityName, id));

            return new Response<bool>(HttpStatusCode.OK, "archived", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<bool>> SetStatusAsync(Guid id, TaskStatus status)
    {
        try
        {
            var entity = await _db.Tasks.Include(t => t.Team).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = user?.FindFirstValue(ClaimTypes.Role);

            if (role == "Worker" && entity.AssignedToId != userId)
                return new Response<bool>(HttpStatusCode.Forbidden, "You are not assigned to this task. Only the owner can change status.");

            entity.Status = status;
            await _db.SaveChangesAsync();

            if (entity.SprintId.HasValue)
            {
                await RecalculateSprintPointsAsync(entity.SprintId.Value);
            }

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            CacheKeyStore.RemoveKey(_cache, EntityName, CacheKeyHelper.BuildIdKey(EntityName, id));

            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetStatusAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> CreateTaskAsync(Application.DTOs.CreateTaskDto dto) => CreateAsync(dto);

    public System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> GetTaskByIdAsync(Guid id) => GetByIdAsync(id);

    public async System.Threading.Tasks.Task<Response<PagedResult<Application.DTOs.GetTaskDto>>> GetTeamTasksAsync(Guid teamId, TaskQueryFilter filter)
    {
        try
        {
            filter ??= new TaskQueryFilter();
            filter.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey($"{EntityName}:team:{teamId}", filter, filter);
            if (_cache.TryGetValue(cacheKey, out PagedResult<Application.DTOs.GetTaskDto> cached))
            {
                return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TaskEntity> query = _db.Tasks.AsNoTracking().Where(x => x.TeamId == teamId);

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = user?.FindFirstValue(ClaimTypes.Role);

            if (role == "Team Lead")
            {
                var team = await _db.Teams.FindAsync(teamId);
                if (team?.TeamLeadId != userId)
                    return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.Forbidden, "Not your team.");
            }

            if (role == "Worker")
            {
                query = query.Where(x => x.AssignedToId == userId);
            }

            if (filter.Status.HasValue) query = query.Where(x => x.Status == filter.Status.Value);
            if (filter.AssigneeId != null) query = query.Where(x => x.AssignedToId == filter.AssigneeId);
            if (filter.SprintId.HasValue) query = query.Where(x => x.SprintId == filter.SprintId.Value);
            if (filter.Priority.HasValue) query = query.Where(x => x.Priority == filter.Priority.Value);
            if (filter.IsBlocked.HasValue) query = query.Where(x => x.IsBlocked == filter.IsBlocked.Value);
            if (filter.IsArchived.HasValue) query = query.Where(x => x.IsArchived == filter.IsArchived.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OrderIndex)
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<Application.DTOs.GetTaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var result = new PagedResult<Application.DTOs.GetTaskDto>
            {
                Items = dtoItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTeamTasksAsync failed");
            return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<PagedResult<Application.DTOs.GetTaskDto>>> GetBacklogTasksAsync(Guid teamId, PaginationFilter filter)
    {
        try
        {
            filter ??= new PaginationFilter();
            filter.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey($"{EntityName}:backlog:{teamId}", new { }, filter);
            if (_cache.TryGetValue(cacheKey, out PagedResult<Application.DTOs.GetTaskDto> cached))
            {
                return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TaskEntity> query = _db.Tasks.AsNoTracking()
                .Where(x => x.TeamId == teamId && x.SprintId == null);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OrderIndex)
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<Application.DTOs.GetTaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var result = new PagedResult<Application.DTOs.GetTaskDto>
            {
                Items = dtoItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBacklogTasksAsync failed");
            return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public System.Threading.Tasks.Task<Response<Application.DTOs.GetTaskDto>> UpdateTaskAsync(Guid id, Application.DTOs.UpdateTaskDto dto) => UpdateAsync(id, dto);

    public System.Threading.Tasks.Task<Response<bool>> UpdateTaskStatusAsync(Guid id, TaskStatus status) => SetStatusAsync(id, status);

    public async System.Threading.Tasks.Task<Response<bool>> AssignTaskAsync(Guid id, string newAssigneeId, string actorId)
    {
        try
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.AssignedToId = newAssigneeId;
            await _db.SaveChangesAsync();

            await RecalculateCapacityAsync(newAssigneeId);

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AssignTaskAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public System.Threading.Tasks.Task<Response<bool>> ReassignTaskAsync(Guid id, string newAssigneeId, string actorId)
    {
        return AssignTaskAsync(id, newAssigneeId, actorId);
    }

    public async System.Threading.Tasks.Task<Response<bool>> SetDeadlineAsync(Guid id, DateTime? deadline)
    {
        try
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.Deadline = deadline;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetDeadlineAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<bool>> SetPriorityAsync(Guid id, TaskPriority priority)
    {
        try
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.Priority = priority;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetPriorityAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<bool>> MoveToSprintAsync(Guid id, Guid? sprintId)
    {
        try
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            var oldSprintId = entity.SprintId;
            entity.SprintId = sprintId;
            await _db.SaveChangesAsync();

            if (oldSprintId.HasValue) await RecalculateSprintPointsAsync(oldSprintId.Value);
            if (sprintId.HasValue) await RecalculateSprintPointsAsync(sprintId.Value);

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MoveToSprintAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public System.Threading.Tasks.Task<Response<bool>> DeleteTaskAsync(Guid id) => DeleteAsync(id);

    public async System.Threading.Tasks.Task<Response<bool>> ReorderTasksAsync(Guid teamId, List<TaskOrderUpdateDto> updates)
    {
        try
        {
            if (updates == null) return new Response<bool>(HttpStatusCode.BadRequest, "updates is null");

            var ids = updates.Select(x => x.Id).ToList();
            var entities = await _db.Tasks.Where(x => x.TeamId == teamId && ids.Contains(x.Id)).ToListAsync();

            foreach (var entity in entities)
            {
                var update = updates.FirstOrDefault(x => x.Id == entity.Id);
                if (update != null) entity.OrderIndex = update.OrderIndex;
            }

            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ReorderTasksAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<PagedResult<Application.DTOs.GetTaskDto>>> GetBlockedTasksAsync(Guid teamId, PaginationFilter filter)
    {
        try
        {
            filter ??= new PaginationFilter();
            filter.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey($"{EntityName}:blocked:{teamId}", new { }, filter);
            if (_cache.TryGetValue(cacheKey, out PagedResult<Application.DTOs.GetTaskDto> cached))
            {
                return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TaskEntity> query = _db.Tasks.AsNoTracking()
                .Where(x => x.TeamId == teamId && x.IsBlocked);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OrderIndex)
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<Application.DTOs.GetTaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var result = new PagedResult<Application.DTOs.GetTaskDto>
            {
                Items = dtoItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBlockedTasksAsync failed");
            return new Response<PagedResult<Application.DTOs.GetTaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<bool>> SetBlockedAsync(Guid id, bool isBlocked, string? reason)
    {
        try
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.IsBlocked = isBlocked;
            entity.BlockedReason = isBlocked ? reason : null;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetBlockedAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<bool>> RejectTaskAsync(Guid id, string actorId, string? reason)
    {
        try
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            var user = _httpContextAccessor.HttpContext?.User;
            var role = user?.FindFirstValue(ClaimTypes.Role);

            if (role == "Worker" && entity.AssignedToId != actorId)
                return new Response<bool>(HttpStatusCode.Forbidden, "Only the assigned worker can reject this task.");

            entity.Status = TaskStatus.Blocked;
            entity.IsBlocked = true;
            entity.BlockedReason = string.IsNullOrWhiteSpace(reason)
                ? "Rejected by assignee"
                : $"Rejected by assignee: {reason}";
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            if (entity.SprintId.HasValue)
            {
                await RecalculateSprintPointsAsync(entity.SprintId.Value);
            }

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "rejected", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RejectTaskAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task RecalculateSprintPointsAsync(Guid sprintId)
    {
        var sprint = await _db.Sprints.FirstOrDefaultAsync(x => x.Id == sprintId);
        if (sprint == null) return;

        var tasks = await _db.Tasks.AsNoTracking().Where(x => x.SprintId == sprintId).ToListAsync();
        sprint.TotalPoints = tasks.Sum(x => x.StoryPoints ?? 0);
        sprint.CompletedPoints = tasks.Where(x => x.Status == TaskStatus.Done).Sum(x => x.StoryPoints ?? 0);
        await _db.SaveChangesAsync();
    }

    private async System.Threading.Tasks.Task RecalculateCapacityAsync(string userId)
    {
        var member = await _db.TeamMembers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (member == null) return;

        var activeTasks = await _db.Tasks.AsNoTracking()
            .CountAsync(x => x.AssignedToId == userId && x.Status != TaskStatus.Done && !x.IsArchived);

        var pct = (int)Math.Round((activeTasks / (double)MaxActiveTasks) * 100);
        member.WeeklyCapacityPct = Math.Max(0, Math.Min(100, pct));
        await _db.SaveChangesAsync();
    }
}
