using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using System.Net;
using Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TaskEntity = Domain.Models.Task;

public class TagService : ITagService
{
    private const string EntityName = "Tag";
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TagService> _logger;

    public TagService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<TagService> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task<Response<GetTagDto>> CreateAsync(InsertTagDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetTagDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<Tag>(dto);
            if (!string.IsNullOrWhiteSpace(entity.Name))
                entity.Name = entity.Name.ToUpperInvariant();

            _db.Tags.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetTagDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTagDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetTagDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<GetTagDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetTagDto cached))
            {
                return new Response<GetTagDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetTagDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetTagDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTagDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetTagDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<PagedResult<GetTagDto>>> GetAllAsync(TagFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new TagFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetTagDto> cached))
            {
                return new Response<PagedResult<GetTagDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<Tag> query = _db.Tags.AsNoTracking();

            if (filter.TeamId.HasValue) query = query.Where(x => x.TeamId == filter.TeamId.Value);
            if (!string.IsNullOrWhiteSpace(filter.Name)) query = query.Where(x => x.Name.Contains(filter.Name));
            if (!string.IsNullOrWhiteSpace(filter.Color)) query = query.Where(x => x.Color == filter.Color);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Name)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetTagDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetTagDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetTagDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetTagDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<GetTagDto>> UpdateAsync(Guid id, UpdateTagDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetTagDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetTagDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            if (!string.IsNullOrWhiteSpace(entity.Name))
                entity.Name = entity.Name.ToUpperInvariant();

            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetTagDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTagDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetTagDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.Tags.Remove(entity);
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

    public async System.Threading.Tasks.Task<Response<List<GetTagDto>>> GetTeamTagsAsync(Guid teamId)
    {
        try
        {
            var key = $"{EntityName}:team:{teamId}";
            if (_cache.TryGetValue(key, out List<GetTagDto> cached))
            {
                return new Response<List<GetTagDto>>(HttpStatusCode.OK, "ok", cached);
            }

            var items = await _db.Tags.AsNoTracking()
                .Where(x => x.TeamId == teamId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetTagDto>>(items);
            _cache.Set(key, dtoItems, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, key);

            return new Response<List<GetTagDto>>(HttpStatusCode.OK, "ok", dtoItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTeamTagsAsync failed");
            return new Response<List<GetTagDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public System.Threading.Tasks.Task<Response<GetTagDto>> CreateTagAsync(Guid teamId, InsertTagDto dto)
    {
        if (dto != null) dto.TeamId = teamId;
        return CreateAsync(dto);
    }

    public System.Threading.Tasks.Task<Response<bool>> DeleteTagAsync(Guid id)
    {
        return DeleteAsync(id);
    }

    public async System.Threading.Tasks.Task<Response<GetTaskTagDto>> AddTagToTaskAsync(Guid taskId, Guid tagId)
    {
        try
        {
            var exists = await _db.TaskTags.AsNoTracking()
                .AnyAsync(x => x.TaskId == taskId && x.TagId == tagId);

            if (exists) return new Response<GetTaskTagDto>(HttpStatusCode.BadRequest, "tag already added");

            var entity = new TaskTag { TaskId = taskId, TagId = tagId, AssignedAt = DateTime.UtcNow };
            _db.TaskTags.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, "TaskTag");
            var dto = _mapper.Map<GetTaskTagDto>(entity);
            return new Response<GetTaskTagDto>(HttpStatusCode.OK, "created", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddTagToTaskAsync failed");
            return new Response<GetTaskTagDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task<Response<bool>> RemoveTagFromTaskAsync(Guid taskId, Guid tagId)
    {
        try
        {
            var entity = await _db.TaskTags.FirstOrDefaultAsync(x => x.TaskId == taskId && x.TagId == tagId);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.TaskTags.Remove(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, "TaskTag");
            return new Response<bool>(HttpStatusCode.OK, "deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveTagFromTaskAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
