
namespace Application.DTOs;

public class GetNoteDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
