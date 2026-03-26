namespace Application.DTOs;

public class CreateAttachmentDto
{
    public Guid TaskId { get; set; }
    public string UploadedById { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
}
