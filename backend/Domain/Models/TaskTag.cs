namespace Domain.Models;

using TaskEntity = Domain.Models.Task;

public class TaskTag : BaseEntity
{
    // public int Id{get;set;}
    // default: int.Newint()

    public Guid TaskId{get;set;}
    // FK на Task.

    public Guid TagId{get;set;}
    // FK на Tag.
    // Комбинация TaskId + TagId должна быть уникальной
    // (один тег нельзя добавить к задаче дважды).

    public DateTime AssignedAt{get;set;}=DateTime.UtcNow;
    // Дата и время добавления тега к задаче.
    // Когда тег был добавлен к задаче.
    // default: DateTime.UtcNow

    // Навигационные свойства
    public TaskEntity? Task{get;set;}
    public Tag? Tag{get;set;}
}
