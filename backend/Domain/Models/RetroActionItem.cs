namespace Domain.Models;

public class RetroActionItem : BaseEntity
{
    // public int Id {get;set;}
    // default: int.Newint()

    public Guid RetroId {get;set;}
    // FK на SprintRetro.

    public string Title {get;set;}= null!;
    // Заголовок пункта действий.
    // Пример: "Infrastructure Review". Max 255 символов.

    public string? Description {get;set;}
    // Описание что нужно сделать. Nullable. Text.

    public ActionItemPriority Priority {get;set;}=ActionItemPriority.Medium;
    // default: ActionItemPriority.Medium
    // Значения: High, Medium, Low

    public DateTime? DueDate {get;set;}
    // Срок выполнения. Nullable.
    // Пример: "By Friday"

    public string? AssignedToId {get;set;}
    // FK на User. Nullable.
    // Кому назначен этот пункт действий.

    public bool IsDone {get;set;}=false;
    // default: false

    // public DateTime CreatedAt {get;set;}
    // default: DateTime.UtcNow

    // Навигационные свойства
    public SprintRetro? Retro {get;set;}
    public ApplicationUser? AssignedTo {get;set;}
}
