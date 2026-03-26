using Domain.Enums;

namespace Domain.Models;

public class Absence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public AbsenceStatus Status { get; set; } = AbsenceStatus.Pending;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public ApplicationUser User { get; set; } = null!;
}
