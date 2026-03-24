namespace Domain.Entities;
public class UserPresence
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = "online"; // online | busy | away
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
