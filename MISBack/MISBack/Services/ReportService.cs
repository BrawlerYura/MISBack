using AutoMapper;
using MISBack.Data;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ReportService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public Task<IcdRootsRepotModel> GetReport(DateTime start, DateTime end, List<string>? icdRoots)
    {
        throw new NotImplementedException();
    }
}