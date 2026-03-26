public class InsertTagDto
{
    public int TeamId { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
}

public class UpdateTagDto : InsertTagDto
{
    public int Id { get; set; }
}

public class GetTagDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
