public class TeamMember : BaseEntity
{
    // Первичный ключ
    // public int Id{get;set;}
    // default: int.Newint()

    public int TeamId{get;set;}
    // FK на Team. Обязателен, не null.

    public int UserId{get;set;}
    // FK на User. Обязателен, не null.

    public DevRole DevRole{get;set;}
    // Роль в разработке. Enum. Обязателен.
    // Значения: Frontend, Backend, Designer,
    //           Tester, DevOps, Fullstack

    public DateTime JoinedAt{get;set;}=DateTime.UtcNow;
    // Дата и время присоединения к команде.
    // default: DateTime.UtcNow при создании записи

    public bool IsActive{get;set;} = true;
    // Указывает активен ли участник в команде.
    // default: true
    // false = участник исключён из команды,
    // но запись не удаляется (история сохраняется)

    public int WeeklyCapacityPct{get;set;} = 0;
    // Загруженность участника в %. 0-100.
    // default: 0
    // Вычисляется: кол-во активных тасков /
    // максимальная нагрузка * 100

    public int? FocusScore{get;set;}
    // Оценка концентрации. 0-100. nullable.
    // default: null (заполняется позже из DaySummary)

    public decimal? ThroughputPtsPerWk{get;set;}
    // Скорость выполнения в story points / неделю.
    // default: null

    // Навигационные свойства
    public Team? Team{get;set;}
    public ApplicationUser? User{get;set;}
    public List<Task>? AssignedTasks{get;set;}
}
