using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class ReportService : IReportService
{
    public Task<IcdRootsRepotModel> GetReport(DateTime start, DateTime end, List<string>? icdRoots)
    {
        throw new NotImplementedException();
    }
}