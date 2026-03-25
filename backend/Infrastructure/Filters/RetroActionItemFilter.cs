public class RetroActionItemFilter
{
    public int? RetroId { get; set; }
    public ActionItemPriority? Priority { get; set; }
    public int? AssignedToId { get; set; }
    public bool? IsDone { get; set; }
    public DateTime? DueFrom { get; set; }
    public DateTime? DueTo { get; set; }
}
