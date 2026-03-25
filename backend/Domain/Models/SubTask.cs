public class Subtask : BaseEntity
{
    // public int Id{get;set;}
    // default: int.Newint()

    public int TaskId{get;set;}
    // FK на Task. Обязателен.

    public string Title{get;set;}=null!;
    // Текст подзадачи. Max 500 символов. Не null.

    public bool IsCompleted{get;set;}=false;
    // default: false

    public int OrderIndex{get;set;}=0;
    // Порядок в чеклисте. default: 0
    // Увеличивается при добавлении новых пунктов.

    public DateTime? CompletedAt{get;set;}
    // Дата/время завершения. Nullable.
    // Устанавливается когда IsCompleted → true.
    // default: null

    // public DateTime CreatedAt{get;set;}
    // default: DateTime.UtcNow

    // Навигационное свойство
    public Task? Task{get;set;}
}