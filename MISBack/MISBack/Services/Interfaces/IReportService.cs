using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IReportService
{
    Task<IcdRootsReportModel> GetReport(DateTime start, DateTime end, List<string>? icdRoots);
}