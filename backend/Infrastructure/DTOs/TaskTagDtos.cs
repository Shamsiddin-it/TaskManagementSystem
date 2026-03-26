public class InsertTaskTagDto
{
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }
}

public class UpdateTaskTagDto : InsertTaskTagDto
{
    public Guid Id { get; set; }
}

public class GetTaskTagDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
