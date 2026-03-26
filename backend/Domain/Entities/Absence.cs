using Domain.Enums;

namespace Domain.Entities;

public class Absence
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public AbsenceStatus Status { get; set; } = AbsenceStatus.Pending;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
