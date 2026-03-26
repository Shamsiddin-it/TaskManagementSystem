using Domain.Enums;

namespace Application.DTOs;

public class UpdateJoinRequestDto
{
    public JoinRequestStatus? Status { get; set; }
    public string? CoverMessage { get; set; }
}
