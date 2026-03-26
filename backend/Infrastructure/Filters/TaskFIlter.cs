public class TaskFilter
{
    public string? Title {get;set;}
    public TaskStatus? Status {get;set;}
    public string? AssignedToUserId {get;set;}
    public TaskPriority? Priority {get;set;}
    public bool? IsDone {get;set;}
    public bool? IsBlocked {get;set;}
    public bool? IsArchived {get;set;}
    public DateTime? DeadlineFrom {get;set;}
    public DateTime? DeadlineTo {get;set;}
    public Guid? SprintId {get;set;}
}