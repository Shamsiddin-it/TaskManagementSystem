namespace Application.DTOs;

public class UpdateAttachmentDto
{
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? FileType { get; set; }
}
