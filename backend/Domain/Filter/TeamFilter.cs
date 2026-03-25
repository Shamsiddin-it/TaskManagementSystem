public class TeamFilter
{
    public Guid? Id { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? TeamLeadId { get; set; }
    public string? Name { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
