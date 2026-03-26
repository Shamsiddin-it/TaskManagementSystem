public class ActivityLogFilter
{
    public int? TeamId { get; set; }
    public int? ActorId { get; set; }
    public ActionType? ActionType { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
