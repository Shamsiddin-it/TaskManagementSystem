public class ProjectCommentFilter
{
    public Guid? Id { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string? Message { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
