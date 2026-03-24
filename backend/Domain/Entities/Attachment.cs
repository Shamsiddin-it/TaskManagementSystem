namespace Domain.Entities;

public class Attachment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UploadedById { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Task Task { get; set; } = null!;
    public User UploadedBy { get; set; } = null!;
}
