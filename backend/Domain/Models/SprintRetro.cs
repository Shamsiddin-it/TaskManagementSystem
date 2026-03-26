public class SprintRetro : BaseEntity
{
    // public int Id{get;set;}
    // default: int.Newint()

    public int SprintId{get;set;}
    // FK на Sprint. Unique — одно ретро на спринт.

    public int CreatedById{get;set;}
    // FK на User (тим лид который создал ретро).

    public int PlannedPoints{get;set;}=0;
    // Запланированные story points спринта.
    // Берётся из Sprint.TotalPoints на момент старта.
    // default: 0

    public int CompletedPoints{get;set;}=0;
    // Выполненные story points.
    // Берётся из Sprint.CompletedPoints.
    // default: 0

    public int SpilloverPoints{get;set;}=0;
    // Невыполненные points = Planned - Completed.
    // Вычисляемое поле, но хранится для истории.
    // default: 0

    public string? WentWell{get;set;}
    // JSON массив строк — что прошло хорошо.
    // Пример: ["Deployments automated","Zero bugs"]
    // Nullable.

    public string? BlockedSummary{get;set;}
    // JSON массив строк — что было заблокировано.
    // Пример: ["GPU latency took 4 days"]
    // Nullable.

    public string? Notes{get;set;}
    // Свободный текст ретроспективных заметок.
    // Textarea из дизайна. Nullable. Text.

    // public DateTime CreatedAt{get;set;}
    // default: DateTime.UtcNow

    // Навигационные свойства
    public Sprint? Sprint{get;set;}
    public ApplicationUser? CreatedBy{get;set;}
    public List<RetroActionItem>? ActionItems{get;set;}
}
