using Domain.Enums;

namespace Application.DTOs;

public class GetAbsenceDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public string Reason { get; set; } = string.Empty;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public AbsenceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
