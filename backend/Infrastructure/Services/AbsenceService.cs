using System.Net;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AbsenceService(ApplicationDbContext dbContext) : IAbsenceService
{
    private readonly ApplicationDbContext context = dbContext;

    public async Task<Response<string>> AddAsync(CreateAbsenceDto dto)
    {
        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var absence = new Absence
        {
            UserId = dto.UserId,
            Reason = dto.Reason,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow
        };

        await context.Absences.AddAsync(absence);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Add Absence successfully");
    }

    public Task<Response<string>> RequestAbsenceAsync(CreateAbsenceDto dto) => AddAsync(dto);

    public async Task<Response<string>> UpdateAsync(int id, UpdateAbsenceDto dto)
    {
        var absence = await context.Absences.FindAsync(id);
        if (absence == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Absence not found");
        }

        if (dto.Reason != null) absence.Reason = dto.Reason;
        if (dto.FromDate.HasValue) absence.FromDate = dto.FromDate.Value;
        if (dto.ToDate.HasValue) absence.ToDate = dto.ToDate.Value;
        if (dto.Status.HasValue) absence.Status = dto.Status.Value;

        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Update Absence successfully");
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var absence = await context.Absences.FindAsync(id);
        if (absence == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Absence not found");
        }

        context.Absences.Remove(absence);
        await context.SaveChangesAsync();
        return new Response<string>(HttpStatusCode.OK, "Delete Absence successfully");
    }

    public async Task<Response<List<GetAbsenceDto>>> GetAllAsync()
    {
        var absences = await context.Absences.Include(x => x.User).ToListAsync();
        var result = absences.Select(absence => new GetAbsenceDto
        {
            Id = absence.Id,
            UserId = absence.UserId,
            User = ServiceMappingHelper.ToGetUserDto(absence.User),
            Reason = absence.Reason,
            FromDate = absence.FromDate,
            ToDate = absence.ToDate,
            Status = absence.Status,
            CreatedAt = absence.CreatedAt
        }).ToList();

        return new Response<List<GetAbsenceDto>>(HttpStatusCode.OK, "ok", result);
    }

    public async Task<Response<GetAbsenceDto>> GetByIdAsync(int id)
    {
        var all = await GetAllAsync();
        var item = all.Date?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return new Response<GetAbsenceDto>(HttpStatusCode.NotFound, "Absence not found");
        }

        return new Response<GetAbsenceDto>(HttpStatusCode.OK, "ok", item);
    }
}
