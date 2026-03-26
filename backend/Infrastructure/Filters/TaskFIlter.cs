public class TaskFilter
{
    public string? Title {get;set;}
    public TaskStatus? Status {get;set;}
    public int? AssignedToUserId {get;set;}
    public TaskPriority? Priority {get;set;}
    public bool? IsDone {get;set;}
    public bool? IsBlocked {get;set;}
    public bool? IsArchived {get;set;}
    public DateTime? DeadlineFrom {get;set;}
    public DateTime? DeadlineTo {get;set;}
    public int? SprintId {get;set;}
}