public class TaskTag : BaseEntity
{
    // public int Id{get;set;}
    // default: int.Newint()

    public int TaskId{get;set;}
    // FK на Task.

    public int TagId{get;set;}
    // FK на Tag.
    // Комбинация TaskId + TagId должна быть уникальной
    // (один тег нельзя добавить к задаче дважды).

    public DateTime AssignedAt{get;set;}=DateTime.UtcNow;
    // Дата и время добавления тега к задаче.
    // Когда тег был добавлен к задаче.
    // default: DateTime.UtcNow

    // Навигационные свойства
    public Task? Task{get;set;}
    public Tag? Tag{get;set;}
}