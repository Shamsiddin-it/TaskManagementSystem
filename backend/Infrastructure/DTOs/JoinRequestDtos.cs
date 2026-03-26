public class GetJoinRequestDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
