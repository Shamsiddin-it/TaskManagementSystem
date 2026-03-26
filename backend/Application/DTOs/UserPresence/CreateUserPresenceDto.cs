using Domain.Enums;

namespace Application.DTOs;
public class CreateUserPresenceDto
{
    public int UserId { get; set; }
    public PresenceStatus Status { get; set; } = PresenceStatus.Online;
}
