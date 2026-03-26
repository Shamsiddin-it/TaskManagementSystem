public class RetroActionItemFilter
{
    public Guid? RetroId { get; set; }
    public ActionItemPriority? Priority { get; set; }
    public string? AssignedToId { get; set; }
    public bool? IsDone { get; set; }
    public DateTime? DueFrom { get; set; }
    public DateTime? DueTo { get; set; }
}
