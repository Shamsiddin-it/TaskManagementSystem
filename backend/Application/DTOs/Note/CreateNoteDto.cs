namespace Application.DTOs;

public class CreateNoteDto
{
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}
