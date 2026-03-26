public class GetJoinRequestDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string UserId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
