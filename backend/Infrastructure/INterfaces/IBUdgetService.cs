public interface IBudgetService
{
    Task<Response<OrgBudgetDto>> GetOrgBudgetAsync(string employerId);
    Task<Response<bool>> UpdateOrgBudgetAsync(string employerId, UpdateOrgBudgetDto dto);
    Task<Response<bool>> AddBudgetRecordAsync(CreateBudgetRecordDto dto);
    Task<Response<List<BudgetRecord>>> GetProjectBudgetHistoryAsync(Guid projectId);
}
