public class TaskItemFilter
{
    public Guid? Id { get; set; }
    public Guid? TeamId { get; set; }
    public string? Title { get; set; }
    public TaskItemStatus? Status { get; set; }
    public bool? IsBlocked { get; set; }
}
