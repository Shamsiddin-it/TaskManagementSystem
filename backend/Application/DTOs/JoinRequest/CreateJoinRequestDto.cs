using Domain.Enums;

namespace Application.DTOs;

public class CreateJoinRequestDto
{
    public Guid TeamId { get; set; }
    public string UserId { get; set; }
    public JoinRequestStatus Status { get; set; } = JoinRequestStatus.Pending;
    public string? CoverMessage { get; set; }
}
