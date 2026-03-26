public interface IReportService
{
    Task<Response<ReportsDashboardDto>> GetDashboardAsync(Guid employerId);
}
