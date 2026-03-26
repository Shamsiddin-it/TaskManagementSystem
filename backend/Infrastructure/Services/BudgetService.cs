using System.Net;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

public class BudgetService : IBudgetService
{
    private readonly ApplicationDbContext _db;
    public BudgetService(ApplicationDbContext db) => _db = db;

    public async Task<Response<OrgBudgetDto>> GetOrgBudgetAsync(string employerId)
    {
        try
        {
            var budget = await _db.OrgBudgets
                .FirstOrDefaultAsync(b => b.EmployerId == employerId);

            if (budget == null)
                return new Response<OrgBudgetDto>(
                    HttpStatusCode.NotFound, "Org budget not found");

            var result = new OrgBudgetDto
            {
                Period = budget.Period,
                TotalBudget = budget.TotalBudget,
                SpentAmount = budget.SpentAmount,
                BurnPercent = budget.BurnPercent,
                EstimatedRunwayMonths = budget.EstimatedRunwayMonths
            };

            return new Response<OrgBudgetDto>(
                HttpStatusCode.OK, "Budget retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<OrgBudgetDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> UpdateOrgBudgetAsync(string employerId, UpdateOrgBudgetDto dto)
    {
        try
        {
            var budget = await _db.OrgBudgets
                .FirstOrDefaultAsync(b => b.EmployerId == employerId);

            if (budget == null)
            {
                budget = new OrgBudget
                {
                    EmployerId = employerId,
                    Period = "Custom",
                    CreatedAt = DateTime.UtcNow
                };
                _db.OrgBudgets.Add(budget);
            }

            if (dto.TotalBudget.HasValue) budget.TotalBudget = dto.TotalBudget.Value;
            if (dto.PeriodStart.HasValue) budget.PeriodStart = dto.PeriodStart.Value;
            if (dto.PeriodEnd.HasValue) budget.PeriodEnd = dto.PeriodEnd.Value;
            if (budget.PeriodStart != default && budget.PeriodEnd != default)
                budget.Period = $"{budget.PeriodStart:yyyy-MM-dd} - {budget.PeriodEnd:yyyy-MM-dd}";

            if (budget.TotalBudget > 0)
            {
                budget.BurnPercent = Math.Round(
                    budget.SpentAmount / budget.TotalBudget * 100, 1);
                decimal remaining = budget.TotalBudget - budget.SpentAmount;
                decimal monthlyBurn = budget.SpentAmount > 0
                    ? budget.SpentAmount / 3 : 1;
                budget.EstimatedRunwayMonths = Math.Round(remaining / monthlyBurn, 1);
            }

            budget.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.OK, "Budget updated successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<bool>> AddBudgetRecordAsync(CreateBudgetRecordDto dto)
    {
        try
        {
            var project = await _db.Projects.FindAsync(dto.ProjectId);
            if (project == null)
                return new Response<bool>(HttpStatusCode.NotFound, "Project not found");

            var record = new BudgetRecord
            {
                ProjectId = dto.ProjectId,
                Description = dto.Description,
                Amount = dto.Amount,
                Type = dto.Type,
                RecordDate = dto.RecordDate,
                CreatedAt = DateTime.UtcNow
            };

            _db.BudgetRecords.Add(record);

            if (dto.Type == BudgetRecordType.Expense)
            {
                project.BudgetSpent = (project.BudgetSpent ?? 0) + dto.Amount;
                project.UpdatedAt = DateTime.UtcNow;

                var orgBudget = await _db.OrgBudgets
                    .FirstOrDefaultAsync(b => b.EmployerId == project.EmployerId);

                if (orgBudget != null)
                {
                    orgBudget.SpentAmount += dto.Amount;
                    orgBudget.BurnPercent = orgBudget.TotalBudget > 0
                        ? Math.Round(orgBudget.SpentAmount / orgBudget.TotalBudget * 100, 1)
                        : 0;
                    orgBudget.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();

            return new Response<bool>(
                HttpStatusCode.Created, "Budget record added successfully", true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<List<BudgetRecord>>> GetProjectBudgetHistoryAsync(Guid projectId)
    {
        try
        {
            var result = await _db.BudgetRecords
                .Where(r => r.ProjectId == projectId)
                .OrderByDescending(r => r.RecordDate)
                .ToListAsync();

            return new Response<List<BudgetRecord>>(
                HttpStatusCode.OK, "Budget history retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<List<BudgetRecord>>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}