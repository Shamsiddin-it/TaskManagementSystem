using Domain.Enums;

namespace Application.DTOs;
public class CreateUserPresenceDto
{
    public string UserId { get; set; }
    public PresenceStatus Status { get; set; } = PresenceStatus.Online;
}
