// From Team Lead module
namespace Domain.Entities;
public class TeamMember
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public string DevRole { get; set; } = "fullstack"; // frontend | backend | designer | tester | devops | fullstack
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
}
