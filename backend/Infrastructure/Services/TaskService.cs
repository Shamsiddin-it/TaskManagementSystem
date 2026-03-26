using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Net;
using System.Threading.Tasks;
using Domain.Models;
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

    public TaskService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<TaskService> logger, ITicketCodeGenerator ticketCodeGenerator)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
        _ticketCodeGenerator = ticketCodeGenerator;
    }

    public async System.Threading.Tasks.Task<Response<GetTaskDto>> CreateAsync(InsertTaskDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetTaskDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<TaskEntity>(dto);
            entity.TicketCode = await _ticketCodeGenerator.GenerateTicketCodeAsync(dto.TeamId, dto.TicketType);
            entity.Status = TaskStatus.Todo;
            entity.IsBlocked = false;
            entity.IsArchived = false;

            _db.Tasks.Add(entity);
            await _db.SaveChangesAsync();

            if (entity.SprintId.HasValue)
            {
                await RecalculateSprintPointsAsync(entity.SprintId.Value);
            }

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetTaskDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTaskDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetTaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<GetTaskDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetTaskDto cached))
            {
                return new Response<GetTaskDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetTaskDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetTaskDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTaskDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetTaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<PagedResult<GetTaskDto>>> GetAllAsync(TaskFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new TaskFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetTaskDto> cached))
            {
                return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.OK, "ok", cached);
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

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OrderIndex)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetTaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetTaskDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<GetTaskDto>> UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetTaskDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetTaskDto>(HttpStatusCode.NotFound, "not found");

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.TeamId = dto.TeamId;
            entity.AssignedToId = dto.AssignedToId;
            entity.CreatedById = dto.CreatedById;
            entity.SprintId = dto.SprintId;
            entity.Priority = dto.Priority;
            entity.TicketType = dto.TicketType;
            entity.Deadline = dto.Deadline;
            entity.EstimatedHours = dto.EstimatedHours;
            entity.StoryPoints = dto.StoryPoints;

            await _db.SaveChangesAsync();

            if (entity.SprintId.HasValue)
            {
                await RecalculateSprintPointsAsync(entity.SprintId.Value);
            }

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetTaskDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTaskDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetTaskDto>(HttpStatusCode.InternalServerError, ex.Message);
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
            var entity = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

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

    public System.Threading.Tasks.Task<Response<GetTaskDto>> CreateTaskAsync(InsertTaskDto dto) => CreateAsync(dto);

    public System.Threading.Tasks.Task<Response<GetTaskDto>> GetTaskByIdAsync(Guid id) => GetByIdAsync(id);

    public async System.Threading.Tasks.Task<Response<PagedResult<GetTaskDto>>> GetTeamTasksAsync(Guid teamId, TaskQueryFilter filter)
    {
        try
        {
            filter ??= new TaskQueryFilter();
            filter.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey($"{EntityName}:team:{teamId}", filter, filter);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetTaskDto> cached))
            {
                return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TaskEntity> query = _db.Tasks.AsNoTracking().Where(x => x.TeamId == teamId);

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

            var dtoItems = _mapper.Map<List<GetTaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var result = new PagedResult<GetTaskDto>
            {
                Items = dtoItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTeamTasksAsync failed");
            return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<PagedResult<GetTaskDto>>> GetBacklogTasksAsync(Guid teamId, PaginationFilter filter)
    {
        try
        {
            filter ??= new PaginationFilter();
            filter.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey($"{EntityName}:backlog:{teamId}", new { }, filter);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetTaskDto> cached))
            {
                return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TaskEntity> query = _db.Tasks.AsNoTracking()
                .Where(x => x.TeamId == teamId && x.SprintId == null);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OrderIndex)
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetTaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var result = new PagedResult<GetTaskDto>
            {
                Items = dtoItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBacklogTasksAsync failed");
            return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public System.Threading.Tasks.Task<Response<GetTaskDto>> UpdateTaskAsync(Guid id, UpdateTaskDto dto) => UpdateAsync(id, dto);

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

    public async System.Threading.Tasks.Task<Response<PagedResult<GetTaskDto>>> GetBlockedTasksAsync(Guid teamId, PaginationFilter filter)
    {
        try
        {
            filter ??= new PaginationFilter();
            filter.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey($"{EntityName}:blocked:{teamId}", new { }, filter);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetTaskDto> cached))
            {
                return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TaskEntity> query = _db.Tasks.AsNoTracking()
                .Where(x => x.TeamId == teamId && x.IsBlocked);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OrderIndex)
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetTaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var result = new PagedResult<GetTaskDto>
            {
                Items = dtoItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBlockedTasksAsync failed");
            return new Response<PagedResult<GetTaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
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
