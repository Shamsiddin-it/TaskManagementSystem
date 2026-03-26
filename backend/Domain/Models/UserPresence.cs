namespace Domain.Models;
public class UserPresence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public string Status { get; set; } = "online"; // online | busy | away
    public DateTime UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
