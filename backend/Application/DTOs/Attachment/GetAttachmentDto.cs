
namespace Application.DTOs;

public class GetAttachmentDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public GetTaskDto Task { get; set; } = null!;
    public string UploadedById { get; set; } = string.Empty;
    public GetUserDto UploadedBy { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
