public class InsertActivityLogDto
{
    public Guid TeamId { get; set; }
    public string ActorId { get; set; } = null!;
    public ActionType ActionType { get; set; }
    public string EntityType { get; set; } = null!;
    public Guid EntityId { get; set; }
    public string? Description { get; set; }
    public string? Metadata { get; set; }
}

public class UpdateActivityLogDto : InsertActivityLogDto
{
    public Guid Id { get; set; }
}

public class GetActivityLogDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string ActorId { get; set; } = null!;
    public ActionType ActionType { get; set; }
    public string EntityType { get; set; } = null!;
    public Guid EntityId { get; set; }
    public string? Description { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
