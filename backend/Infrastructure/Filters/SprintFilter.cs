public class SprintFilter
{
    public Guid? TeamId { get; set; }
    public SprintStatus? Status { get; set; }
    public DateTime? StartFrom { get; set; }
    public DateTime? StartTo { get; set; }
    public DateTime? EndFrom { get; set; }
    public DateTime? EndTo { get; set; }
    public int? Number { get; set; }
}
