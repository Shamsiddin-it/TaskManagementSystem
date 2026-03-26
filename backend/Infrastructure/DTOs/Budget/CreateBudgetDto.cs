public class CreateBudgetRecordDto
{
    public int ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BudgetRecordType Type { get; set; }
    public DateTime RecordDate { get; set; }
}
