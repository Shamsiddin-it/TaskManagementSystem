using Domain.Enums;

namespace Application.DTOs;

public class CreateAbsenceDto
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public AbsenceStatus Status { get; set; } = AbsenceStatus.Pending;
}
