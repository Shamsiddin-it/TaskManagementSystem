using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class TaskTagService : ITaskTagService
{
    private const string EntityName = "TaskTag";
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TaskTagService> _logger;

    public TaskTagService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<TaskTagService> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Response<GetTaskTagDto>> CreateAsync(InsertTaskTagDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetTaskTagDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<TaskTag>(dto);
            _db.TaskTags.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetTaskTagDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTaskTagDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetTaskTagDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetTaskTagDto>> GetByIdAsync(int id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetTaskTagDto cached))
            {
                return new Response<GetTaskTagDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.TaskTags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetTaskTagDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetTaskTagDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTaskTagDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetTaskTagDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<PagedResult<GetTaskTagDto>>> GetAllAsync(TaskTagFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new TaskTagFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetTaskTagDto> cached))
            {
                return new Response<PagedResult<GetTaskTagDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TaskTag> query = _db.TaskTags.AsNoTracking();

            if (filter.TaskId.HasValue) query = query.Where(x => x.TaskId == filter.TaskId.Value);
            if (filter.TagId.HasValue) query = query.Where(x => x.TagId == filter.TagId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetTaskTagDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetTaskTagDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetTaskTagDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetTaskTagDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetTaskTagDto>> UpdateAsync(int id, UpdateTaskTagDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetTaskTagDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.TaskTags.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetTaskTagDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetTaskTagDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTaskTagDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetTaskTagDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _db.TaskTags.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.TaskTags.Remove(entity);
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
}
