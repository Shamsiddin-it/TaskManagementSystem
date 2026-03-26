public class GetJoinRequestDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string UserId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
