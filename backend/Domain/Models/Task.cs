public class Task : BaseEntity
{
    // public int Id{get;set;}
    // default: int.Newint()

    public string TicketCode{get;set;} = null!;
    // Уникальный код тикета: "API-92", "UI-214".
    // Генерируется автоматически при создании.
    // Формат: [PREFIX]-[NUMBER]
    // Prefix берётся из типа задачи или проекта.
    // Не null, уникальный в рамках команды.

    public string Title{get;set;} = null!;
    // Заголовок задачи. Не null. Max 255 символов.

    public string? Description{get;set;}
    // Описание задачи. Nullable. Text (без лимита).

    public int TeamId{get;set;}
    // FK на Team. Обязателен.

    public int AssignedToId{get;set;}
    // FK на User — кто выполняет задачу.
    // Обязателен при создании.

    public int CreatedById{get;set;}
    // FK на User — кто создал (тим лид).
    // Обязателен. Устанавливается из текущего юзера.

    public int? SprintId{get;set;}
    // FK на Sprint. Nullable.
    // null = задача в бэклоге, не в спринте.

    public TaskStatus Status{get;set;}=TaskStatus.Todo;
    // default: TaskStatus.Todo
    // Значения: Todo, InProgress, Review, Done, Blocked

    public TaskPriority Priority{get;set;}=TaskPriority.Medium;
    // default: TaskPriority.Medium
    // Значения: Low, Medium, High, Critical

    public TicketType TicketType{get;set;}=TicketType.Task;
    // default: TicketType.Task
    // Значения: Feature, Bug, Task, Docs, QA, Infra

    public DateTime? Deadline{get;set;}
    // Дедлайн задачи. Nullable.
    // default: null

    public int? EstimatedHours{get;set;}
    // Оценочное время в часах. Nullable.
    // default: null

    public int? StoryPoints{get;set;}
    // Story points по шкале Fibonacci.
    // Nullable. Используется для подсчёта
    // velocity спринта.
    // default: null

    public int OrderIndex{get;set;}=0;
    // Порядок карточки в канбан-колонке.
    // default: 0
    // При drag-and-drop обновляется.

    public bool IsBlocked{get;set;}=false;
    // Задача заблокирована.
    // default: false
    // true = появляется в Blocked Items Triage.

    public string? BlockedReason{get;set;}
    // Причина блокировки. Nullable.
    // Заполняется когда IsBlocked = true.

    public int TotalTimeMinutes{get;set;}=0;
    // Суммарное время потраченное на задачу.
    // default: 0
    // Обновляется при завершении FocusSession.

    public bool IsArchived{get;set;}=false;
    // Задача в архиве.
    // default: false

    // public DateTime CreatedAt{get;set;}
    // default: DateTime.UtcNow

    // public DateTime UpdatedAt{get;set;}
    // default: DateTime.UtcNow
    // Обновляется при каждом изменении.

    // Навигационные свойства
    public Team? Team{get;set;}
    public ApplicationUser? AssignedTo{get;set;}
    public ApplicationUser? CreatedBy{get;set;}
    public Sprint? Sprint{get;set;}
    public List<Subtask>? Subtasks{get;set;}
    public List<TaskComment>? Comments{get;set;}
    public List<TaskTag>? TaskTags{get;set;}
    public List<TaskAttachment>? Attachments{get;set;}
    public List<TaskTimeLog>? TimeLogs{get;set;}
    public List<ActivityLog>? ActivityLogs{get;set;}
}
