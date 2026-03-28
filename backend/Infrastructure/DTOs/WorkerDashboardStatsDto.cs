using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class WorkerDashboardStatsDto
    {
        public int CompletedToday { get; set; }
        public int CurrentStreak { get; set; }
        public decimal FocusHours { get; set; }
        public int DailyGoalHours { get; set; }
        public int DailyGoalProgressPercent { get; set; }
        public string FocusStatus { get; set; } = string.Empty;

        public List<DailyLoadDto> WeeklyLoadDistribution { get; set; } = new();
    }

    public class DailyLoadDto
    {
        public string Day { get; set; } = string.Empty;
        public decimal EstimatedHours { get; set; }
        public int TaskCount { get; set; }
        public decimal FocusTimeLogged { get; set; }
    }

    public class WorkerActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }
}
