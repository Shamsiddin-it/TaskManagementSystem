public class TaskQueryFilter : PaginationFilter
{
    public TaskStatus? Status { get; set; }
    public int? AssigneeId { get; set; }
    public int? SprintId { get; set; }
    public TaskPriority? Priority { get; set; }
    public bool? IsBlocked { get; set; }
    public bool? IsArchived { get; set; }
}
