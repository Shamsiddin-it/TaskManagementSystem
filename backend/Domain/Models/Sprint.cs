namespace Domain.Models;

public class Sprint : BaseEntity
{
    // public int Id{get;set;}
    // default: int.Newint()

    public Guid TeamId{get;set;}
    // FK на Team. Обязателен.

    public string Name{get;set;}=null!;
    // Название спринта. Например "Sprint 43".
    // Генерируется автоматически или вводится вручную.
    // Max 100 символов.

    public int Number{get;set;}
    //serial number of sprint in team. Auto-increment per team: 1, 2, 3...
    // Порядковый номер спринта в команде.
    // Автоинкремент внутри команды: 1, 2, 3...
    // Уникальный в рамках команды.

    public SprintStatus Status{get;set;}=SprintStatus.Planning;
    // default: SprintStatus.Planning
    // Значения: Planning, Active, Completed, Archived

    public DateTime StartDate{get;set;}
    // Дата начала спринта. Обязательна.

    public DateTime EndDate{get;set;}
    // Дата конца спринта. Обязательна.
    // Должна быть > StartDate.

    public int TotalPoints{get;set;}=0;
    // Суммарные story points всех задач
    // в спринте на данный момент.
    // default: 0
    // Пересчитывается при добавлении/удалении тасков.

    public int CompletedPoints{get;set;}=0;
    // Завершённые story points (таски в Done).
    // default: 0

    public int CapacityPoints{get;set;}=40;
    // Максимальная ёмкость спринта.
    // default: 40 (стандартные 2 недели)
    // Устанавливается тим лидом вручную.

    public string? Goal{get;set;}
    // Цель спринта. Nullable.
    // Короткое описание чего хотим достичь.

    // public DateTime CreatedAt{get;set;}
    // default: DateTime.UtcNow

    // Навигационные свойства
    public Team? Team{get;set;}
    public List<Task>? Tasks{get;set;}
    public SprintRetro? Retrospective{get;set;}
}
