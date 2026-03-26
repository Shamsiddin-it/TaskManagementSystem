namespace Application.DTOs;

public class CreateNoteDto
{
    public string UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}
