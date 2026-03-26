using System.Net;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _db;

    public ReportService(ApplicationDbContext db) => _db = db;

    public async Task<Response<ReportsDashboardDto>> GetDashboardAsync(string employerId)
    {
        try
        {
            var now = DateTime.UtcNow;
            var months = Enumerable.Range(0, 6)
                .Select(offset => new DateTime(now.Year, now.Month, 1).AddMonths(-(5 - offset)))
                .ToList();

            var projects = await _db.Projects
                .Where(p => p.EmployerId == employerId)
                .ToListAsync();

            var projectIds = projects.Select(p => p.Id).ToList();

            var budgetRecords = await _db.BudgetRecords
                .Where(r => projectIds.Contains(r.ProjectId) && r.Type == BudgetRecordType.Expense)
                .ToListAsync();

            var membersByMonth = await _db.ProjectMembers
                .Where(m => projectIds.Contains(m.ProjectId))
                .ToListAsync();

            var completionTrend = months.Select(month =>
            {
                var monthProjects = projects
                    .Where(p => p.CreatedAt <= month.AddMonths(1).AddTicks(-1))
                    .ToList();

                var actual = monthProjects.Count == 0
                    ? 0m
                    : (decimal)Math.Round(monthProjects.Average(p => p.CompletionPercent), 1);

                return new ReportsTrendPointDto
                {
                    Label = month.ToString("MMM", System.Globalization.CultureInfo.InvariantCulture).ToUpper(),
                    Actual = actual,
                    Target = Math.Min(actual + 8m, 100m)
                };
            }).ToList();

            var projectSpend = budgetRecords
                .GroupBy(r => r.ProjectId)
                .Select(g => new ProjectSpendSummaryDto
                {
                    ProjectId = g.Key,
                    ProjectTitle = projects.First(p => p.Id == g.Key).Title,
                    Amount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .Take(5)
                .ToList();

            var heatmap = new List<int>();
            for (var week = 0; week < 4; week++)
            {
                for (var day = 0; day < 7; day++)
                {
                    var from = now.Date.AddDays(-27 + (week * 7) + day);
                    var to = from.AddDays(1);

                    var activity = membersByMonth.Count(m => m.JoinedAt >= from && m.JoinedAt < to)
                                   + budgetRecords.Count(r => r.CreatedAt >= from && r.CreatedAt < to);

                    heatmap.Add(Math.Min(activity, 4));
                }
            }

            var result = new ReportsDashboardDto
            {
                CompletionTrend = completionTrend,
                BudgetSpendByProject = projectSpend,
                TeamHeatmap = heatmap
            };

            return new Response<ReportsDashboardDto>(
                HttpStatusCode.OK, "Reports dashboard retrieved successfully", result);
        }
        catch (Exception ex)
        {
            return new Response<ReportsDashboardDto>(
                HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
