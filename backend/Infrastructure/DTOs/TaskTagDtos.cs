public class InsertTaskTagDto
{
    public int TaskId { get; set; }
    public int TagId { get; set; }
}

public class UpdateTaskTagDto : InsertTaskTagDto
{
    public int Id { get; set; }
}

public class GetTaskTagDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int TagId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
