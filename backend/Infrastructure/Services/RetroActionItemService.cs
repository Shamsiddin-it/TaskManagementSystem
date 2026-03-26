using System.Net;
using Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RetroActionItemService : IRetroActionItemService
{
    private const string EntityName = "RetroActionItem";
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RetroActionItemService> _logger;

    public RetroActionItemService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<RetroActionItemService> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Response<GetRetroActionItemDto>> CreateAsync(InsertRetroActionItemDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<RetroActionItem>(dto);
            _db.RetroActionItems.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetRetroActionItemDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetRetroActionItemDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetRetroActionItemDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetRetroActionItemDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetRetroActionItemDto cached))
            {
                return new Response<GetRetroActionItemDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.RetroActionItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetRetroActionItemDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetRetroActionItemDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetRetroActionItemDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<PagedResult<GetRetroActionItemDto>>> GetAllAsync(RetroActionItemFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new RetroActionItemFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetRetroActionItemDto> cached))
            {
                return new Response<PagedResult<GetRetroActionItemDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<RetroActionItem> query = _db.RetroActionItems.AsNoTracking();

            if (filter.RetroId.HasValue) query = query.Where(x => x.RetroId == filter.RetroId.Value);
            if (filter.Priority.HasValue) query = query.Where(x => x.Priority == filter.Priority.Value);
            if (!string.IsNullOrEmpty(filter.AssignedToId)) query = query.Where(x => x.AssignedToId == filter.AssignedToId);
            if (filter.IsDone.HasValue) query = query.Where(x => x.IsDone == filter.IsDone.Value);
            if (filter.DueFrom.HasValue) query = query.Where(x => x.DueDate >= filter.DueFrom.Value);
            if (filter.DueTo.HasValue) query = query.Where(x => x.DueDate <= filter.DueTo.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetRetroActionItemDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetRetroActionItemDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetRetroActionItemDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetRetroActionItemDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetRetroActionItemDto>> UpdateAsync(Guid id, UpdateRetroActionItemDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.RetroActionItems.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetRetroActionItemDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetRetroActionItemDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetRetroActionItemDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetRetroActionItemDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _db.RetroActionItems.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.RetroActionItems.Remove(entity);
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

    public async Task<Response<bool>> SetDoneAsync(Guid id, bool isDone)
    {
        try
        {
            var entity = await _db.RetroActionItems.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.IsDone = isDone;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            CacheKeyStore.RemoveKey(_cache, EntityName, CacheKeyHelper.BuildIdKey(EntityName, id));

            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetDoneAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
