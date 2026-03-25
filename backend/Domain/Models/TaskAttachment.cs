public class TaskAttachment : BaseEntity
{
    public int TaskId { get; set; }
    public string FileName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public long SizeBytes { get; set; }
    public Task? Task { get; set; }
}
