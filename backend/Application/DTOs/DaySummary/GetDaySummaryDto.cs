namespace Application.DTOs;

public class GetDaySummaryDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
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
}
