public class ActivityLogFilter
{
    public Guid? TeamId { get; set; }
    public string? ActorId { get; set; }
    public ActionType? ActionType { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
