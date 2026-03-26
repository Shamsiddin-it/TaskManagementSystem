public class InsertTagDto
{
    public Guid TeamId { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
}

public class UpdateTagDto : InsertTagDto
{
    public Guid Id { get; set; }
}

public class GetTagDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
