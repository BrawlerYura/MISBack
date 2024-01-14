using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
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

    public async Task<IcdRootsReportModel> GetReport(DateTime start, DateTime end, List<string>? icdRoots)
    {
        var icdRootsReportModel = new IcdRootsReportModel
        {
            Filters = new IcdRootsReportFiltersModel
            {
                Start = start,
                End = end,
                IcdRoots = icdRoots
            }
        };
        
        var inspectionList = await _context.Inspection.Where(i => i.Date <= end & i.Date >= start).ToListAsync();
        var uniquePatientsId = new HashSet<Patient>();
        var icdRootsReportRecordModelList = new List<IcdRootsReportRecordModel>();
        var summaryByRoot = new Dictionary<string, int>();
        foreach (var inspection in inspectionList)
        {
            var patientEntity = await _context.Patient.FirstOrDefaultAsync(p => p.Id == inspection.PatientId);
            
            var inspectionDiagnosis = await _context.InspectionDiagnosis
                .Where(id => id.InspectionId == inspection.Id).FirstOrDefaultAsync();
            if (inspectionDiagnosis == null)
            {
                throw new KeyNotFoundException($"diagnoses not found in inspection with id {inspection.Id}");
            }

            var diagnosis =
                await _context.Diagnosis.FirstOrDefaultAsync(d => d.Id == inspectionDiagnosis.DiagnosisId);
            if (diagnosis == null)
            {
                throw new KeyNotFoundException($"diagnoses not found in inspection with id {inspection.Id}");
            }

            var icd = await _context.Icd10.Where(i => i.MkbCode == diagnosis.Code).FirstOrDefaultAsync();
            if (icd == null)
            {
                throw new KeyNotFoundException($"icd10 not found with code {diagnosis.Code}");
            }

            var rootIcd = await _context.Icd10.FirstOrDefaultAsync(ri => ri.RecCode == icd.RecCode.Substring(0, 2));
            if (rootIcd == null)
            {
                throw new KeyNotFoundException($"icd10 root not found with rec code {icd.RecCode}");
            }

            if (!icdRoots.Any(ir => ir == rootIcd.MkbCode) & icdRoots != null & icdRoots.Count > 0)
            {
                continue;
            }
            
            if (uniquePatientsId.Add(patientEntity))
            {
                var icdRootsReportRecordModel = new IcdRootsReportRecordModel
                {
                    PatientName = patientEntity.Name,
                    PatientId = patientEntity.Id,
                    Gender = patientEntity.Gender,
                    PatientBirthdate = patientEntity.Birthday,
                    VisitsByRoot = new Dictionary<string, int> { { rootIcd.MkbCode, 1 } }
                };

                icdRootsReportRecordModelList.Add(icdRootsReportRecordModel);
            }
            else
            {
                var icdRootsReportRecordModel =
                    icdRootsReportRecordModelList.FirstOrDefault(i => i.PatientId == inspection.PatientId);
                
                if (icdRootsReportRecordModel.VisitsByRoot.ContainsKey(rootIcd.MkbCode))
                {
                    icdRootsReportRecordModel.VisitsByRoot[rootIcd.MkbCode]++;
                }
                else
                {
                    icdRootsReportRecordModel.VisitsByRoot.Add(rootIcd.MkbCode, 1);
                }
            }
            
            if (summaryByRoot.ContainsKey(rootIcd.MkbCode))
            {
                summaryByRoot[rootIcd.MkbCode]++;
            }
            else
            {
                summaryByRoot.Add(rootIcd.MkbCode, 1);
            }
        }

        icdRootsReportModel.SummaryByRoot = summaryByRoot;
        icdRootsReportModel.Records = icdRootsReportRecordModelList;

        return icdRootsReportModel;
    }
}