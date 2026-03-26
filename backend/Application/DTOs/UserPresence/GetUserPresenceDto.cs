using Domain.Enums;

namespace Application.DTOs;

public class GetUserPresenceDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public PresenceStatus Status { get; set; }
    public DateTime UpdatedAt { get; set; }
}
