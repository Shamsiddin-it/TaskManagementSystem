public interface IReportService
{
    Task<Response<ReportsDashboardDto>> GetDashboardAsync(string employerId);
}
