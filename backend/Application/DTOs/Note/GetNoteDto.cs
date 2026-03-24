
namespace Application.DTOs;

public class GetNoteDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public GetUserDto User { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
