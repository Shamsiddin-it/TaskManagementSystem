public class InsertActivityLogDto
{
    public int TeamId { get; set; }
    public string ActorId { get; set; } = null!;
    public ActionType ActionType { get; set; }
    public string EntityType { get; set; } = null!;
    public int EntityId { get; set; }
    public string? Description { get; set; }
    public string? Metadata { get; set; }
}

public class UpdateActivityLogDto : InsertActivityLogDto
{
    public int Id { get; set; }
}

public class GetActivityLogDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string ActorId { get; set; } = null!;
    public ActionType ActionType { get; set; }
    public string EntityType { get; set; } = null!;
    public int EntityId { get; set; }
    public string? Description { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
