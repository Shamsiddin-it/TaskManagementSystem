using Domain.Enums;

namespace Application.DTOs;

public class GetUserPresenceDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public PresenceStatus Status { get; set; }
    public DateTime UpdatedAt { get; set; }
}
