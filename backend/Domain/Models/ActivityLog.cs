public class ActivityLog : BaseEntity
{
    // public int Id{get;set;}
    // default: int.Newint()

    public int TeamId{get;set;}
    // FK на Team. К какой команде относится событие.

    public int ActorId{get;set;}
    // FK на User — кто совершил действие.

    public ActionType ActionType{get;set;}
    // Тип действия. Обязателен.
    // Значения: TaskMoved, TaskCreated, TaskAssigned,
    //           TaskCommented, SprintStarted,
    //           SprintCompleted, MemberJoined,
    //           MemberRemoved

    public string EntityType{get;set;}=null!;
    // Тип сущности над которой совершено действие.
    // Например: "Task", "Sprint", "TeamMember".
    // Max 50 символов.

    public int EntityId{get;set;}
    // ID конкретной сущности.
    // Например, ID задачи которую переместили.

    public string? Description{get;set;}
    // Читаемое описание события для ленты активности.
    // Пример: "Sarah Chen moved UI-214 to In Review"
    // Max 500 символов.

    public string? Metadata{get;set;}
    // Дополнительные данные в JSON. Nullable.
    // Пример: {"from":"Todo","to":"InProgress"}
    // Используется для детальных отчётов и аналитики.

    // public DateTime CreatedAt{get;set;}
    // default: DateTime.UtcNow

    // Навигационные свойства
    public Team? Team{get;set;}
    public ApplicationUser? Actor{get;set;}
}
