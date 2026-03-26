using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class SprintRetroService : ISprintRetroService
{
    private const string EntityName = "SprintRetro";
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SprintRetroService> _logger;

    public SprintRetroService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<SprintRetroService> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Response<GetSprintRetroDto>> CreateAsync(InsertSprintRetroDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetSprintRetroDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<SprintRetro>(dto);
            _db.SprintRetros.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetSprintRetroDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSprintRetroDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetSprintRetroDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetSprintRetroDto>> GetByIdAsync(int id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetSprintRetroDto cached))
            {
                return new Response<GetSprintRetroDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.SprintRetros.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetSprintRetroDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetSprintRetroDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSprintRetroDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetSprintRetroDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<PagedResult<GetSprintRetroDto>>> GetAllAsync(SprintRetroFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new SprintRetroFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetSprintRetroDto> cached))
            {
                return new Response<PagedResult<GetSprintRetroDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<SprintRetro> query = _db.SprintRetros.AsNoTracking();

            if (filter.SprintId.HasValue) query = query.Where(x => x.SprintId == filter.SprintId.Value);
            if (filter.CreatedById.HasValue) query = query.Where(x => x.CreatedById == filter.CreatedById.Value);
            if (filter.CreatedFrom.HasValue) query = query.Where(x => x.CreatedAt >= filter.CreatedFrom.Value);
            if (filter.CreatedTo.HasValue) query = query.Where(x => x.CreatedAt <= filter.CreatedTo.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetSprintRetroDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetSprintRetroDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetSprintRetroDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetSprintRetroDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetSprintRetroDto>> UpdateAsync(int id, UpdateSprintRetroDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetSprintRetroDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.SprintRetros.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetSprintRetroDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetSprintRetroDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSprintRetroDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetSprintRetroDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _db.SprintRetros.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.SprintRetros.Remove(entity);
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

    public async Task<Response<GetSprintRetroDto>> CreateRetroAsync(int sprintId, InsertSprintRetroDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetSprintRetroDto>(HttpStatusCode.BadRequest, "dto is null");

            var sprint = await _db.Sprints.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sprintId);
            if (sprint == null) return new Response<GetSprintRetroDto>(HttpStatusCode.NotFound, "sprint not found");
            if (sprint.Status != SprintStatus.Completed) return new Response<GetSprintRetroDto>(HttpStatusCode.BadRequest, "sprint not completed");

            var entity = _mapper.Map<SprintRetro>(dto);
            entity.SprintId = sprintId;
            entity.PlannedPoints = sprint.TotalPoints;
            entity.CompletedPoints = sprint.CompletedPoints;
            entity.SpilloverPoints = Math.Max(0, sprint.TotalPoints - sprint.CompletedPoints);

            _db.SprintRetros.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetSprintRetroDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetSprintRetroDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateRetroAsync failed");
            return new Response<GetSprintRetroDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetSprintRetroDto>> GetRetroAsync(int sprintId)
    {
        try
        {
            var key = $"{EntityName}:sprint:{sprintId}";
            if (_cache.TryGetValue(key, out GetSprintRetroDto cached))
            {
                return new Response<GetSprintRetroDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.SprintRetros.AsNoTracking().FirstOrDefaultAsync(x => x.SprintId == sprintId);
            if (entity == null) return new Response<GetSprintRetroDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetSprintRetroDto>(entity);
            _cache.Set(key, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, key);

            return new Response<GetSprintRetroDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetRetroAsync failed");
            return new Response<GetSprintRetroDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public Task<Response<GetSprintRetroDto>> UpdateRetroAsync(int id, UpdateSprintRetroDto dto)
    {
        return UpdateAsync(id, dto);
    }

    public async Task<Response<GetRetroActionItemDto>> CreateActionItemAsync(int retroId, InsertRetroActionItemDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.BadRequest, "dto is null");

            var retro = await _db.SprintRetros.AsNoTracking().FirstOrDefaultAsync(x => x.Id == retroId);
            if (retro == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.NotFound, "retro not found");

            var entity = _mapper.Map<RetroActionItem>(dto);
            entity.RetroId = retroId;

            _db.RetroActionItems.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, "RetroActionItem");
            var resDto = _mapper.Map<GetRetroActionItemDto>(entity);
            return new Response<GetRetroActionItemDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateActionItemAsync failed");
            return new Response<GetRetroActionItemDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetRetroActionItemDto>> UpdateActionItemAsync(int id, UpdateRetroActionItemDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.RetroActionItems.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, "RetroActionItem");
            var resDto = _mapper.Map<GetRetroActionItemDto>(entity);
            return new Response<GetRetroActionItemDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateActionItemAsync failed");
            return new Response<GetRetroActionItemDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetRetroActionItemDto>> ToggleActionItemAsync(int id)
    {
        try
        {
            var entity = await _db.RetroActionItems.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.NotFound, "not found");

            entity.IsDone = !entity.IsDone;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, "RetroActionItem");
            var resDto = _mapper.Map<GetRetroActionItemDto>(entity);
            return new Response<GetRetroActionItemDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ToggleActionItemAsync failed");
            return new Response<GetRetroActionItemDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteActionItemAsync(int id)
    {
        try
        {
            var entity = await _db.RetroActionItems.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.RetroActionItems.Remove(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, "RetroActionItem");
            return new Response<bool>(HttpStatusCode.OK, "deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteActionItemAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
