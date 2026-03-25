using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class SprintService : ISprintService
{
    private const string EntityName = "Sprint";
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SprintService> _logger;

    public SprintService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<SprintService> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Response<GetSprintDto>> CreateAsync(InsertSprintDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetSprintDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<Sprint>(dto);
            _db.Sprints.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetSprintDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSprintDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetSprintDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetSprintDto>> GetByIdAsync(int id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetSprintDto cached))
            {
                return new Response<GetSprintDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.Sprints.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetSprintDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetSprintDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSprintDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetSprintDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<PagedResult<GetSprintDto>>> GetAllAsync(SprintFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new SprintFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetSprintDto> cached))
            {
                return new Response<PagedResult<GetSprintDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<Sprint> query = _db.Sprints.AsNoTracking();

            if (filter.TeamId.HasValue) query = query.Where(x => x.TeamId == filter.TeamId.Value);
            if (filter.Status.HasValue) query = query.Where(x => x.Status == filter.Status.Value);
            if (filter.Number.HasValue) query = query.Where(x => x.Number == filter.Number.Value);
            if (filter.StartFrom.HasValue) query = query.Where(x => x.StartDate >= filter.StartFrom.Value);
            if (filter.StartTo.HasValue) query = query.Where(x => x.StartDate <= filter.StartTo.Value);
            if (filter.EndFrom.HasValue) query = query.Where(x => x.EndDate >= filter.EndFrom.Value);
            if (filter.EndTo.HasValue) query = query.Where(x => x.EndDate <= filter.EndTo.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.StartDate)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetSprintDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetSprintDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetSprintDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetSprintDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetSprintDto>> UpdateAsync(int id, UpdateSprintDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetSprintDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.Sprints.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetSprintDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetSprintDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSprintDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetSprintDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _db.Sprints.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.Sprints.Remove(entity);
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

    public async Task<Response<bool>> SetStatusAsync(int id, SprintStatus status)
    {
        try
        {
            var entity = await _db.Sprints.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.Status = status;
            await _db.SaveChangesAsync();

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

    public async Task<Response<GetSprintDto>> CreateSprintAsync(int teamId, InsertSprintDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetSprintDto>(HttpStatusCode.BadRequest, "dto is null");

            var lastNumber = await _db.Sprints.AsNoTracking()
                .Where(x => x.TeamId == teamId)
                .OrderByDescending(x => x.Number)
                .Select(x => x.Number)
                .FirstOrDefaultAsync();

            var entity = _mapper.Map<Sprint>(dto);
            entity.TeamId = teamId;
            entity.Number = lastNumber + 1;
            entity.Status = SprintStatus.Planning;

            _db.Sprints.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetSprintDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSprintDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateSprintAsync failed");
            return new Response<GetSprintDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<PagedResult<GetSprintDto>>> GetSprintsAsync(int teamId, SprintQueryFilter filter)
    {
        try
        {
            filter ??= new SprintQueryFilter();
            filter.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey($"{EntityName}:team:{teamId}", filter, filter);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetSprintDto> cached))
            {
                return new Response<PagedResult<GetSprintDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<Sprint> query = _db.Sprints.AsNoTracking().Where(x => x.TeamId == teamId);
            if (filter.Status.HasValue) query = query.Where(x => x.Status == filter.Status.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.StartDate)
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetSprintDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var result = new PagedResult<GetSprintDto>
            {
                Items = dtoItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetSprintDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSprintsAsync failed");
            return new Response<PagedResult<GetSprintDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetSprintDto>> GetActiveSprintAsync(int teamId)
    {
        try
        {
            var key = $"{EntityName}:active:{teamId}";
            if (_cache.TryGetValue(key, out GetSprintDto cached))
            {
                return new Response<GetSprintDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.Sprints.AsNoTracking()
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.Status == SprintStatus.Active);

            if (entity == null) return new Response<GetSprintDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetSprintDto>(entity);
            _cache.Set(key, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, key);

            return new Response<GetSprintDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetActiveSprintAsync failed");
            return new Response<GetSprintDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public Task<Response<GetSprintDto>> GetSprintByIdAsync(int id)
    {
        return GetByIdAsync(id);
    }

    public async Task<Response<bool>> StartSprintAsync(int id, int actorId)
    {
        try
        {
            var sprint = await _db.Sprints.FirstOrDefaultAsync(x => x.Id == id);
            if (sprint == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            var hasActive = await _db.Sprints.AsNoTracking()
                .AnyAsync(x => x.TeamId == sprint.TeamId && x.Status == SprintStatus.Active);

            if (hasActive) return new Response<bool>(HttpStatusCode.BadRequest, "active sprint exists");

            sprint.Status = SprintStatus.Active;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            CacheKeyStore.RemoveKey(_cache, EntityName, CacheKeyHelper.BuildIdKey(EntityName, id));

            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartSprintAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> CompleteSprintAsync(int id)
    {
        try
        {
            var sprint = await _db.Sprints.FirstOrDefaultAsync(x => x.Id == id);
            if (sprint == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            sprint.Status = SprintStatus.Completed;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            CacheKeyStore.RemoveKey(_cache, EntityName, CacheKeyHelper.BuildIdKey(EntityName, id));

            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CompleteSprintAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public Task<Response<GetSprintDto>> UpdateSprintAsync(int id, UpdateSprintDto dto)
    {
        return UpdateAsync(id, dto);
    }

    public async Task<Response<SprintStatsDto>> GetSprintStatsAsync(int id)
    {
        try
        {
            var sprint = await _db.Sprints.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (sprint == null) return new Response<SprintStatsDto>(HttpStatusCode.NotFound, "not found");

            var stats = new SprintStatsDto
            {
                SprintId = sprint.Id,
                PlannedPoints = sprint.TotalPoints,
                CompletedPoints = sprint.CompletedPoints,
                SpilloverPoints = Math.Max(0, sprint.TotalPoints - sprint.CompletedPoints),
                Velocity = sprint.CompletedPoints
            };

            return new Response<SprintStatsDto>(HttpStatusCode.OK, "ok", stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSprintStatsAsync failed");
            return new Response<SprintStatsDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
