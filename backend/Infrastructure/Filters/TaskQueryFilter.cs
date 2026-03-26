public class TaskQueryFilter : PaginationFilter
{
    public TaskStatus? Status { get; set; }
    public string? AssigneeId { get; set; }
    public Guid? SprintId { get; set; }
    public TaskPriority? Priority { get; set; }
    public bool? IsBlocked { get; set; }
    public bool? IsArchived { get; set; }
}
