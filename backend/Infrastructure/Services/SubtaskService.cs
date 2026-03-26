using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class SubtaskService : ISubtaskService
{
    private const string EntityName = "Subtask";
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SubtaskService> _logger;

    public SubtaskService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<SubtaskService> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Response<GetSubtaskDto>> CreateAsync(InsertSubtaskDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetSubtaskDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<Subtask>(dto);
            _db.Subtasks.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetSubtaskDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSubtaskDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetSubtaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetSubtaskDto>> GetByIdAsync(int id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetSubtaskDto cached))
            {
                return new Response<GetSubtaskDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.Subtasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetSubtaskDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetSubtaskDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSubtaskDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetSubtaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<PagedResult<GetSubtaskDto>>> GetAllAsync(SubtaskFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new SubtaskFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetSubtaskDto> cached))
            {
                return new Response<PagedResult<GetSubtaskDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<Subtask> query = _db.Subtasks.AsNoTracking();

            if (filter.TaskId.HasValue) query = query.Where(x => x.TaskId == filter.TaskId.Value);
            if (filter.IsCompleted.HasValue) query = query.Where(x => x.IsCompleted == filter.IsCompleted.Value);
            if (filter.CompletedFrom.HasValue) query = query.Where(x => x.CompletedAt >= filter.CompletedFrom.Value);
            if (filter.CompletedTo.HasValue) query = query.Where(x => x.CompletedAt <= filter.CompletedTo.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.OrderIndex)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetSubtaskDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetSubtaskDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetSubtaskDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetSubtaskDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetSubtaskDto>> UpdateAsync(int id, UpdateSubtaskDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetSubtaskDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.Subtasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetSubtaskDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetSubtaskDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSubtaskDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetSubtaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _db.Subtasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.Subtasks.Remove(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            CacheKeyStore.RemoveKey(_cache, EntityName, CacheKeyHelper.BuildIdKey(EntityName, id));

            return new Response<bool>(HttpStatusCode.OK, "deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> SetCompletedAsync(int id, bool isCompleted)
    {
        try
        {
            var entity = await _db.Subtasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.IsCompleted = isCompleted;
            entity.CompletedAt = isCompleted ? DateTime.UtcNow : null;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            CacheKeyStore.RemoveKey(_cache, EntityName, CacheKeyHelper.BuildIdKey(EntityName, id));

            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetCompletedAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public Task<Response<GetSubtaskDto>> CreateSubtaskAsync(int taskId, InsertSubtaskDto dto)
    {
        if (dto != null) dto.TaskId = taskId;
        return CreateAsync(dto);
    }

    public Task<Response<GetSubtaskDto>> UpdateSubtaskAsync(int id, UpdateSubtaskDto dto)
    {
        return UpdateAsync(id, dto);
    }

    public async Task<Response<GetSubtaskDto>> ToggleSubtaskAsync(int id)
    {
        try
        {
            var entity = await _db.Subtasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetSubtaskDto>(HttpStatusCode.NotFound, "not found");

            entity.IsCompleted = !entity.IsCompleted;
            entity.CompletedAt = entity.IsCompleted ? DateTime.UtcNow : null;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var resDto = _mapper.Map<GetSubtaskDto>(entity);
            return new Response<GetSubtaskDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ToggleSubtaskAsync failed");
            return new Response<GetSubtaskDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public Task<Response<bool>> DeleteSubtaskAsync(int id)
    {
        return DeleteAsync(id);
    }

    public async Task<Response<bool>> ReorderSubtasksAsync(int taskId, List<SubtaskOrderUpdateDto> updates)
    {
        try
        {
            if (updates == null) return new Response<bool>(HttpStatusCode.BadRequest, "updates is null");

            var ids = updates.Select(x => x.Id).ToList();
            var entities = await _db.Subtasks.Where(x => x.TaskId == taskId && ids.Contains(x.Id)).ToListAsync();

            foreach (var entity in entities)
            {
                var update = updates.FirstOrDefault(x => x.Id == entity.Id);
                if (update != null)
                {
                    entity.OrderIndex = update.OrderIndex;
                }
            }

            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ReorderSubtasksAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
