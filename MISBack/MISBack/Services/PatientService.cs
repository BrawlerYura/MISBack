using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PatientService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<Guid> CreatePatient(PatientCreateModel patientCreateModel)
    {
        if (patientCreateModel.Birthday >= DateTime.Now)
        {
            throw new BadHttpRequestException("Birth date mustn't be later then now");
        }

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.Now,
            Name = patientCreateModel.Name,
            Birthday = patientCreateModel.Birthday,
            Gender = patientCreateModel.Gender
        };
            
        await _context.Patient.AddAsync(patient);

        return patient.Id;
    }

    public Task<PatientPagedListModel> GetPatientsList(string? name, List<string> conclusions, PatientSorting sorting, bool scheduledVisits = false,
        bool onlyMine = false, int page = 1, int size = 5)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> CreateInspectionForPatient(Guid patientId, Guid doctorId, InspectionCreateModel inspectionCreateModel)
    {
        var inspectionEntity = _mapper.Map<Inspection>(inspectionCreateModel);
        
        inspectionEntity.Id = Guid.NewGuid();
        inspectionEntity.CreateTime = DateTime.Now;
        inspectionEntity.PatientId = patientId;
        inspectionEntity.DoctorId = doctorId;
        
        if (inspectionEntity.PreviousInspectionId == null)
        {
            inspectionEntity.BaseInspectionId = null;
        }
        else
        {
            var previousInspection = await _context.Inspection.Where(i => i.Id == inspectionEntity.PreviousInspectionId)
                .FirstOrDefaultAsync();
            if (previousInspection == null)
            {
                throw new KeyNotFoundException($"inspection with id {inspectionEntity.PreviousInspectionId} not found");
            }

            inspectionEntity.BaseInspectionId = previousInspection.BaseInspectionId;
        }

        await _context.Inspection.AddAsync(inspectionEntity);
        
        await CreateDiagnoses(inspectionCreateModel.Diagnoses, inspectionEntity.Id);

        if (inspectionCreateModel.Consultations != null)
        {
            await CreateConsultations(inspectionCreateModel.Consultations, doctorId);
        }

        return inspectionEntity.Id;
    }

    public Task<InspectionPagedListModel> GetPatientsInspectionsList(Guid patientId, List<string>? icdRoots, bool grouped = false, int page = 1, int size = 5)
    {
        throw new NotImplementedException();
    }

    public async Task<PatientModel> GetPatientCard(Guid patientId)
    {
        var patientEntity = await _context.Patient.Where(p => p.Id == patientId).FirstOrDefaultAsync();

        if (patientEntity == null)
        {
            throw new KeyNotFoundException($"patient with id {patientId} not found");
        }

        return _mapper.Map<PatientModel>(patientEntity);
    }

    public Task<PatientModel> SearchPatientMedicalInspections(Guid patientId, string? request)
    {
        throw new NotImplementedException();
    }

    private async Task CreateDiagnoses(List<DiagnosisCreateModel> diagnoses, Guid inspectionId)
    {
        foreach (var diagnosis in diagnoses)
        {
            var diagnosisIcd10 =
                await _context.Icd10.Where(d => d.Id == diagnosis.IcdDiagnosisId).FirstOrDefaultAsync();

            if (diagnosisIcd10 == null)
            {
                throw new KeyNotFoundException($"diagnosis with id {diagnosis.IcdDiagnosisId} not found in icd10");
            }

            var newDiagnosis = new Diagnosis
            {
                Id = new Guid(),
                CreateTime = DateTime.Now,
                Code = diagnosisIcd10.Code,
                Description = diagnosis.Description,
                Name = diagnosisIcd10.Name,
                Type = diagnosis.Type
            };

            await _context.Diagnosis.AddAsync(newDiagnosis);
            
            await _context.InspectionDiagnosis.AddAsync(new InspectionDiagnosis
            {
                InspectionId = inspectionId,
                DiagnosisId = newDiagnosis.Id
            });
        }
    }

    private async Task CreateConsultations(List<ConsultationCreateModel> consultations, Guid doctorId)
    {
        foreach (var consultation in consultations)
        {
            var consultationEntity = new Consultation
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                InspectionId = consultation.InspectionId,
                SpecialityId = consultation.SpecialityId
            };

            await _context.Consultation.AddAsync(consultationEntity);

            var rootComment = new Comment
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                AuthorId = doctorId,
                ConsultationId = consultationEntity.Id,
                Content = consultation.Comment.Content,
                IsRootComment = true
            };

            await _context.Comment.AddAsync(rootComment);
        }
    }
}