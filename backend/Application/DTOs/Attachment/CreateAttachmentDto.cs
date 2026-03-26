namespace Application.DTOs;

public class CreateAttachmentDto
{
    public int TaskId { get; set; }
    public int UploadedById { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
}
