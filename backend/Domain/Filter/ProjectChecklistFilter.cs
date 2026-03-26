public class ProjectChecklistFilter
{
    public Guid? Id { get; set; }
    public Guid? ProjectId { get; set; }
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
    public bool? IsBlocked { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
