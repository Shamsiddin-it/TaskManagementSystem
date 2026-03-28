using Infrastructure.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Domain.Enums;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/worker-dashboard")]
    public class WorkerDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkerDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        [Authorize]
        public async Task<IActionResult> GetStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var today = DateTime.UtcNow.Date;

            // Tasks completed today
            var completedTodayCount = await _context.Tasks
                .Where(t => t.AssignedToId == userId && t.Status == TaskStatus.Done && t.UpdatedAt.Date == today)
                .CountAsync();

            // Focus Hours today
            var focusSessionsToday = await _context.FocusSessions
                .Where(fs => fs.UserId == userId && fs.StartedAt.Date == today)
                .ToListAsync();
            
            var focusMinutesLogged = focusSessionsToday.Sum(fs => fs.DurationMinutes ?? 0);
            var focusHours = Math.Round((decimal)focusMinutesLogged / 60, 1);
            
            // Build weekly scale: 7 days ending today
            var weekStart = today.AddDays(-6);
            
            var weekTasks = await _context.Tasks
                .Where(t => t.AssignedToId == userId && t.UpdatedAt.Date >= weekStart)
                .ToListAsync();

            var weekFocus = await _context.FocusSessions
                .Where(fs => fs.UserId == userId && fs.StartedAt.Date >= weekStart)
                .ToListAsync();

            var weeklyLoad = new List<DailyLoadDto>();
            for (int i = 0; i <= 6; i++)
            {
                var targetDate = weekStart.AddDays(i);

                var tasksThatDay = weekTasks.Where(t => t.UpdatedAt.Date == targetDate.Date).ToList();
                var focusThatDay = weekFocus.Where(fs => fs.StartedAt.Date == targetDate.Date).ToList();

                weeklyLoad.Add(new DailyLoadDto
                {
                    Day = targetDate.ToString("ddd").ToUpper(),
                    EstimatedHours = tasksThatDay.Sum(t => (decimal)(t.EstimatedHours ?? 0)),
                    TaskCount = tasksThatDay.Count,
                    FocusTimeLogged = Math.Round((decimal)focusThatDay.Sum(fs => fs.DurationMinutes ?? 0) / 60, 1),
                });
            }

            var dailyGoalHours = 8;
            var progress = focusHours >= dailyGoalHours ? 100 : (int)(((double)focusHours / dailyGoalHours) * 100);

            var stats = new WorkerDashboardStatsDto
            {
                CompletedToday = completedTodayCount,
                CurrentStreak = 0,
                FocusHours = focusHours,
                DailyGoalHours = dailyGoalHours,
                DailyGoalProgressPercent = progress,
                FocusStatus = "Active",
                WeeklyLoadDistribution = weeklyLoad
            };

            return Ok(stats);
        }

        [HttpGet("activities")]
        [Authorize]
        public async Task<IActionResult> GetActivities()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var recentTasks = await _context.Tasks
                .Where(t => t.AssignedToId == userId)
                .OrderByDescending(t => t.UpdatedAt)
                .Take(5)
                .Select(t => new WorkerActivityDto
                {
                    Title = t.Status == TaskStatus.Done ? "Completed Task" : "Updated Task",
                    Description = t.Title,
                    Timestamp = t.UpdatedAt,
                    ProjectName = "Workspace"
                })
                .ToListAsync();

            if (!recentTasks.Any())
            {
                recentTasks.Add(new WorkerActivityDto { Title = "Started Session", Description = "API initialized", Timestamp = DateTime.UtcNow, ProjectName = "System" });
            }

            return Ok(recentTasks);
        }
    }
}
