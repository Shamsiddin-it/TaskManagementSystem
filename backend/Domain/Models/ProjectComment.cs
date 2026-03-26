using Domain.Models;

public class ProjectComment : BaseEntity
{
    // public Guid Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;
    public Guid? ParentCommentId { get; set; }       // для тредов (ответ на комментарий)
    public ProjectComment? ParentComment { get; set; }
    public string Message { get; set; } = string.Empty;
    // public DateTime CreatedAt { get; set; }
}
