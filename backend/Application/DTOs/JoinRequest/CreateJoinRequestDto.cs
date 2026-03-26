using Domain.Enums;

namespace Application.DTOs;

public class CreateJoinRequestDto
{
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public JoinRequestStatus Status { get; set; } = JoinRequestStatus.Pending;
    public string? CoverMessage { get; set; }
}
