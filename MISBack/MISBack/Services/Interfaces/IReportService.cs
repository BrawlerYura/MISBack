using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IReportService
{
    Task<IcdRootsRepotModel> GetReport(DateTime start, DateTime end, List<string>? icdRoots);
}