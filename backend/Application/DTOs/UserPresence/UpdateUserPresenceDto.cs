using Domain.Enums;

namespace Application.DTOs;

public class UpdateUserPresenceDto
{
    public PresenceStatus? Status { get; set; }
}
