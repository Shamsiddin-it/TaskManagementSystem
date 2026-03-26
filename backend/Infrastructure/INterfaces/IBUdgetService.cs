public interface IBudgetService
{
    Task<Response<OrgBudgetDto>> GetOrgBudgetAsync(Guid employerId);
    Task<Response<bool>> UpdateOrgBudgetAsync(Guid employerId, UpdateOrgBudgetDto dto);
    Task<Response<bool>> AddBudgetRecordAsync(CreateBudgetRecordDto dto);
    Task<Response<List<BudgetRecord>>> GetProjectBudgetHistoryAsync(Guid projectId);
}
