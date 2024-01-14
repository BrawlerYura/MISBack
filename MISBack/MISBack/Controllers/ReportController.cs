using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers;

[ApiController]
[Route("api/report/icdrootsreport")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IcdRootsReportModel> GetReport([FromQuery] DateTime start, [FromQuery] DateTime end,
        [FromQuery] List<string>? icdRoots)
    {
        return await _reportService.GetReport(start, end, icdRoots);
    }
}