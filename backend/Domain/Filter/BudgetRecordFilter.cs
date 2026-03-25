public class BudgetRecordFilter
{
    public Guid? Id { get; set; }
    public Guid? ProjectId { get; set; }
    public string? Description { get; set; }
    public BudgetRecordType? Type { get; set; }
    public decimal? AmountFrom { get; set; }
    public decimal? AmountTo { get; set; }
    public DateTime? RecordDateFrom { get; set; }
    public DateTime? RecordDateTo { get; set; }
}
