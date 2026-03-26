using System.Net;
using Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class ActivityLogService : IActivityLogService
{
    private const string EntityName = "ActivityLog";
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<ActivityLogService> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Response<GetActivityLogDto>> CreateAsync(InsertActivityLogDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetActivityLogDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<ActivityLog>(dto);
            _db.ActivityLogs.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetActivityLogDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetActivityLogDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetActivityLogDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetActivityLogDto>> GetByIdAsync(int id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetActivityLogDto cached))
            {
                return new Response<GetActivityLogDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.ActivityLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetActivityLogDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetActivityLogDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetActivityLogDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetActivityLogDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<PagedResult<GetActivityLogDto>>> GetAllAsync(ActivityLogFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new ActivityLogFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetActivityLogDto> cached))
            {
                return new Response<PagedResult<GetActivityLogDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<ActivityLog> query = _db.ActivityLogs.AsNoTracking();

            if (filter.TeamId.HasValue) query = query.Where(x => x.TeamId == filter.TeamId.Value);
            if (filter.ActorId.HasValue) query = query.Where(x => x.ActorId == filter.ActorId.Value);
            if (filter.ActionType.HasValue) query = query.Where(x => x.ActionType == filter.ActionType.Value);
            if (!string.IsNullOrWhiteSpace(filter.EntityType)) query = query.Where(x => x.EntityType.Contains(filter.EntityType));
            if (filter.EntityId.HasValue) query = query.Where(x => x.EntityId == filter.EntityId.Value);
            if (filter.CreatedFrom.HasValue) query = query.Where(x => x.CreatedAt >= filter.CreatedFrom.Value);
            if (filter.CreatedTo.HasValue) query = query.Where(x => x.CreatedAt <= filter.CreatedTo.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetActivityLogDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetActivityLogDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetActivityLogDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetActivityLogDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetActivityLogDto>> UpdateAsync(int id, UpdateActivityLogDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetActivityLogDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.ActivityLogs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetActivityLogDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetActivityLogDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetActivityLogDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetActivityLogDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _db.ActivityLogs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.ActivityLogs.Remove(entity);
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

    public async Task<Response<List<GetActivityLogDto>>> GetTeamActivityAsync(int teamId, LimitOffsetFilter filter)
    {
        try
        {
            filter ??= new LimitOffsetFilter();
            filter.Normalize();

            var key = $"ActivityLog:team:{teamId}:{filter.Offset}:{filter.Limit}";
            if (_cache.TryGetValue(key, out List<GetActivityLogDto> cached))
            {
                return new Response<List<GetActivityLogDto>>(HttpStatusCode.OK, "ok", cached);
            }

            var items = await _db.ActivityLogs.AsNoTracking()
                .Where(x => x.TeamId == teamId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(filter.Offset)
                .Take(filter.Limit)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetActivityLogDto>>(items);
            _cache.Set(key, dtoItems, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, key);

            return new Response<List<GetActivityLogDto>>(HttpStatusCode.OK, "ok", dtoItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTeamActivityAsync failed");
            return new Response<List<GetActivityLogDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<List<GetActivityLogDto>>> GetTaskActivityAsync(int taskId, LimitOffsetFilter filter)
    {
        try
        {
            filter ??= new LimitOffsetFilter();
            filter.Normalize();

            var key = $"ActivityLog:task:{taskId}:{filter.Offset}:{filter.Limit}";
            if (_cache.TryGetValue(key, out List<GetActivityLogDto> cached))
            {
                return new Response<List<GetActivityLogDto>>(HttpStatusCode.OK, "ok", cached);
            }

            var items = await _db.ActivityLogs.AsNoTracking()
                .Where(x => x.EntityType == "Task" && x.EntityId == taskId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(filter.Offset)
                .Take(filter.Limit)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetActivityLogDto>>(items);
            _cache.Set(key, dtoItems, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, key);

            return new Response<List<GetActivityLogDto>>(HttpStatusCode.OK, "ok", dtoItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTaskActivityAsync failed");
            return new Response<List<GetActivityLogDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public Task<Response<GetActivityLogDto>> CreateActivityLogAsync(InsertActivityLogDto dto)
    {
        return CreateAsync(dto);
    }
}
