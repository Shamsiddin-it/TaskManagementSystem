public class UserFilter
{
    public Guid? Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public Domain.Enums.UserRole? Role { get; set; }
    public OnlineStatus? OnlineStatus { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
