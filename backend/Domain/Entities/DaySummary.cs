namespace Domain.Entities;

public class DaySummary
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime SummaryDate { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksTotal { get; set; }
    public decimal FocusHours { get; set; }
    public int ProductivityScore { get; set; }
    public string? ProductivityGrade { get; set; }
    public int StreakDays { get; set; }
    public string? TomorrowPriority1 { get; set; }
    public string? TomorrowPriority2 { get; set; }
    public string? TomorrowPriority3 { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
