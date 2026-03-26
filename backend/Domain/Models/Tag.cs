namespace Domain.Models;

public class Tag : BaseEntity
{
    // public int Id{get;set;}
    // default: int.Newint()

    public Guid TeamId{get;set;}
    // FK на Team. Теги создаются на уровне команды,
    // не глобально. Каждая команда имеет свои.

    public string Name{get;set;}
    // Название тега. Например "UI-UX", "FRONTEND".
    // Уникальный в рамках команды.
    // Хранится в UPPER CASE. Max 50 символов.

    public string? Color{get;set;}
    // Hex цвет тега для отображения.
    // Nullable. Пример: "#B475FF".
    // default: null (фронт использует дефолтный цвет)

    // public DateTime CreatedAt{get;set;}
    // default: DateTime.UtcNow

    // Навигационные свойства
    public Team? Team{get;set;}
    public List<TaskTag>? TaskTags{get;set;}
}
