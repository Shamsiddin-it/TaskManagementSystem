public class BudgetRecord : BaseEntity
{
    // public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BudgetRecordType Type { get; set; }   // allocation | expense | adjustment
    public DateTime RecordDate { get; set; }
    // public DateTime CreatedAt { get; set; }
}
