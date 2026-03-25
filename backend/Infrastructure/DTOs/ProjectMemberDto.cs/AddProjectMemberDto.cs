public class AddProjectMemberDto
{
    public Guid UserId { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
}
