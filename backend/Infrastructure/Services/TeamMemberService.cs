using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TaskEntity = Domain.Models.Task;

public class TeamMemberService : ITeamMemberService
{
    private const string EntityName = "TeamMember";
    private const int MaxActiveTasks = 10;
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TeamMemberService> _logger;

    public TeamMemberService(ApplicationDbContext db, IMapper mapper, IMemoryCache cache, ILogger<TeamMemberService> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Response<GetTeamMemberDto>> CreateAsync(InsertTeamMemberDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetTeamMemberDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = _mapper.Map<TeamMember>(dto);
            _db.TeamMembers.Add(entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetTeamMemberDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTeamMemberDto>(HttpStatusCode.OK, "created", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return new Response<GetTeamMemberDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetTeamMemberDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, id);
            if (_cache.TryGetValue(idKey, out GetTeamMemberDto cached))
            {
                return new Response<GetTeamMemberDto>(HttpStatusCode.OK, "ok", cached);
            }

            var entity = await _db.TeamMembers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetTeamMemberDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetTeamMemberDto>(entity);
            _cache.Set(idKey, dto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTeamMemberDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed");
            return new Response<GetTeamMemberDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<PagedResult<GetTeamMemberDto>>> GetAllAsync(TeamMemberFilter filter, PaginationFilter pagination)
    {
        try
        {
            filter ??= new TeamMemberFilter();
            pagination ??= new PaginationFilter();
            pagination.Normalize();

            var cacheKey = CacheKeyHelper.BuildListKey(EntityName, filter, pagination);
            if (_cache.TryGetValue(cacheKey, out PagedResult<GetTeamMemberDto> cached))
            {
                return new Response<PagedResult<GetTeamMemberDto>>(HttpStatusCode.OK, "ok", cached);
            }

            IQueryable<TeamMember> query = _db.TeamMembers.AsNoTracking();

            if (filter.TeamId.HasValue) query = query.Where(x => x.TeamId == filter.TeamId.Value);
            if (!string.IsNullOrEmpty(filter.UserId)) query = query.Where(x => x.UserId == filter.UserId);
            if (filter.DevRole.HasValue) query = query.Where(x => x.DevRole == filter.DevRole.Value);
            if (filter.IsActive.HasValue) query = query.Where(x => x.IsActive == filter.IsActive.Value);
            if (filter.JoinedFrom.HasValue) query = query.Where(x => x.JoinedAt >= filter.JoinedFrom.Value);
            if (filter.JoinedTo.HasValue) query = query.Where(x => x.JoinedAt <= filter.JoinedTo.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.JoinedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetTeamMemberDto>>(items);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<GetTeamMemberDto>
            {
                Items = dtoItems,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            _cache.Set(cacheKey, result, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, cacheKey);

            return new Response<PagedResult<GetTeamMemberDto>>(HttpStatusCode.OK, "ok", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllAsync failed");
            return new Response<PagedResult<GetTeamMemberDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetTeamMemberDto>> UpdateAsync(Guid id, UpdateTeamMemberDto dto)
    {
        try
        {
            if (dto == null) return new Response<GetTeamMemberDto>(HttpStatusCode.BadRequest, "dto is null");

            var entity = await _db.TeamMembers.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<GetTeamMemberDto>(HttpStatusCode.NotFound, "not found");

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            var idKey = CacheKeyHelper.BuildIdKey(EntityName, entity.Id);
            var resDto = _mapper.Map<GetTeamMemberDto>(entity);
            _cache.Set(idKey, resDto, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, idKey);

            return new Response<GetTeamMemberDto>(HttpStatusCode.OK, "updated", resDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return new Response<GetTeamMemberDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _db.TeamMembers.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            _db.TeamMembers.Remove(entity);
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

    public async Task<Response<bool>> SetActiveAsync(Guid id, bool isActive)
    {
        try
        {
            var entity = await _db.TeamMembers.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            entity.IsActive = isActive;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            CacheKeyStore.RemoveKey(_cache, EntityName, CacheKeyHelper.BuildIdKey(EntityName, id));

            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetActiveAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<List<GetTeamMemberDto>>> GetTeamMembersAsync(Guid teamId)
    {
        try
        {
            var key = $"{EntityName}:team:{teamId}:active";
            if (_cache.TryGetValue(key, out List<GetTeamMemberDto> cached))
            {
                return new Response<List<GetTeamMemberDto>>(HttpStatusCode.OK, "ok", cached);
            }

            var items = await _db.TeamMembers.AsNoTracking()
                .Where(x => x.TeamId == teamId && x.IsActive)
                .OrderBy(x => x.UserId)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<GetTeamMemberDto>>(items);
            _cache.Set(key, dtoItems, CacheKeyHelper.DefaultOptions());
            CacheKeyStore.Add(EntityName, key);

            return new Response<List<GetTeamMemberDto>>(HttpStatusCode.OK, "ok", dtoItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTeamMembersAsync failed");
            return new Response<List<GetTeamMemberDto>>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetTeamMemberDto>> GetMemberProfileAsync(Guid teamId, string userId)
    {
        try
        {
            var member = await _db.TeamMembers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.UserId == userId);

            if (member == null) return new Response<GetTeamMemberDto>(HttpStatusCode.NotFound, "not found");

            var dto = _mapper.Map<GetTeamMemberDto>(member);
            return new Response<GetTeamMemberDto>(HttpStatusCode.OK, "ok", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetMemberProfileAsync failed");
            return new Response<GetTeamMemberDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> AssignDevRoleAsync(Guid teamId, string userId, DevRole role, string actorId)
    {
        try
        {
            var member = await _db.TeamMembers.FirstOrDefaultAsync(x => x.TeamId == teamId && x.UserId == userId);
            if (member == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            member.DevRole = role;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AssignDevRoleAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> RemoveMemberAsync(Guid teamId, string userId, string actorId)
    {
        try
        {
            var member = await _db.TeamMembers.FirstOrDefaultAsync(x => x.TeamId == teamId && x.UserId == userId);
            if (member == null) return new Response<bool>(HttpStatusCode.NotFound, "not found");

            member.IsActive = false;
            await _db.SaveChangesAsync();

            CacheKeyStore.RemoveEntity(_cache, EntityName);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveMemberAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public System.Threading.Tasks.Task<Response<bool>> AcceptJoinRequestAsync(Guid joinRequestId)
    {
        return System.Threading.Tasks.Task.FromResult(new Response<bool>(HttpStatusCode.NotImplemented, "JoinRequest not implemented"));
    }

    public System.Threading.Tasks.Task<Response<bool>> RejectJoinRequestAsync(Guid joinRequestId)
    {
        return System.Threading.Tasks.Task.FromResult(new Response<bool>(HttpStatusCode.NotImplemented, "JoinRequest not implemented"));
    }

    public System.Threading.Tasks.Task<Response<List<GetJoinRequestDto>>> GetJoinRequestsAsync(Guid teamId, PaginationFilter filter)
    {
        return System.Threading.Tasks.Task.FromResult(new Response<List<GetJoinRequestDto>>(HttpStatusCode.NotImplemented, "JoinRequest not implemented"));
    }

    public async Task<Response<bool>> UpdateCapacityAsync(Guid teamId, string userId)
    {
        try
        {
            await RecalculateCapacityAsync(userId);
            return new Response<bool>(HttpStatusCode.OK, "updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateCapacityAsync failed");
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async System.Threading.Tasks.Task RecalculateCapacityAsync(string userId)
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
