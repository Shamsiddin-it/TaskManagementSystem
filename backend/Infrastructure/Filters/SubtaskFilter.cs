public class SubtaskFilter
{
    public Guid? TaskId { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTime? CompletedFrom { get; set; }
    public DateTime? CompletedTo { get; set; }
}
