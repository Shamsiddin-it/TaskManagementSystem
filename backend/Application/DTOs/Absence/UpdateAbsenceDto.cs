using Domain.Enums;

namespace Application.DTOs;

public class UpdateAbsenceDto
{
    public string? Reason { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public AbsenceStatus? Status { get; set; }
}
