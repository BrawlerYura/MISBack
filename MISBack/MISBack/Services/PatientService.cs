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
        if (patientCreateModel.Birthday >= DateTime.UtcNow)
        {
            throw new BadHttpRequestException("Birth date mustn't be later then now");
        }

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            Name = patientCreateModel.Name,
            Birthday = patientCreateModel.Birthday,
            Gender = patientCreateModel.Gender
        };

        await _context.Patient.AddAsync(patient);
        await _context.SaveChangesAsync();

        return patient.Id;
    }

    public async Task<PatientPagedListModel> GetPatientsList(Guid doctorId, string? name, List<Conclusion>? conclusions,
        PatientSorting sorting, bool scheduledVisits = false,
        bool onlyMine = false, int page = 1, int size = 5)
    {
        var query = _context.Patient.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(patient => patient.Name.Contains(name));
        }

        if (conclusions is { Count: > 0 })
        {
            query = query
                .Where(patient => conclusions.Any(
                    conclusion => _context.Inspection
                        .Any(inspection => inspection.PatientId == patient.Id && inspection.Conclusion == conclusion)));
        }

        if (scheduledVisits)
        {
            query = query
                .Where(patient => _context.Inspection
                    .Where(inspection => inspection.PatientId == patient.Id)
                    .Any(inspection => inspection.NextVisitDate >= DateTime.UtcNow));
        }

        if (onlyMine)
        {
            query = query.Where(patient => _context.Inspection
                .Where(inspection => inspection.PatientId == patient.Id)
                .Any(inspection => inspection.DoctorId == doctorId)
            );
        }

        switch (sorting)
        {
            case PatientSorting.NameAsc:
                query = query.OrderBy(patient => patient.Name);
                break;
            case PatientSorting.NameDesc:
                query = query.OrderByDescending(patient => patient.Name);
                break;
            case PatientSorting.CreateAsc:
                query = query.OrderBy(patient => patient.CreateTime);
                break;
            case PatientSorting.CreateDesc:
                query = query.OrderByDescending(patient => patient.CreateTime);
                break;
            case PatientSorting.InspectionAsc:
                query = query.OrderBy(patient => _context.Inspection
                    .Where(i => i.PatientId == patient.Id).OrderBy(i => i.Date));
                break;
            case PatientSorting.InspectionDesc:
                query = query.OrderByDescending(patient => _context.Inspection
                    .Where(i => i.PatientId == patient.Id).OrderByDescending(i => i.Date));
                break;
        }

        var count = (int)Math.Ceiling((double)query.Count() / (double)size);
        if (count == 0)
        {
            throw new BadHttpRequestException("no users were found with this request");
        }

        var patients = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var patientListModel = CreatePatientModel(patients);

        var patientPagedListModel = new PatientPagedListModel
        {
            Patients = patientListModel,
            Pagination = new PageInfoModel
            {
                Size = patientListModel.Count,
                Count = count,
                Current = page
            }
        };

        if (page < 1 || page > count)
        {
            throw new BadHttpRequestException("Invalid value for attribute page");
        }

        return patientPagedListModel;
    }

    public async Task<Guid> CreateInspectionForPatient(Guid patientId, Guid doctorId,
        InspectionCreateModel inspectionCreateModel)
    {
        await ValidateInspection(inspectionCreateModel, patientId);

        var inspectionEntity = _mapper.Map<Inspection>(inspectionCreateModel);

        inspectionEntity.Id = Guid.NewGuid();
        inspectionEntity.CreateTime = DateTime.UtcNow;
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
        await _context.SaveChangesAsync();
        await CreateDiagnoses(inspectionCreateModel.Diagnoses, inspectionEntity.Id);

        if (inspectionCreateModel.Consultations != null)
        {
            await CreateConsultations(inspectionCreateModel.Consultations, doctorId, inspectionEntity.Id);
        }

        return inspectionEntity.Id;
    }

    public Task<InspectionPagedListModel> GetPatientsInspectionsList(Guid patientId, List<string>? icdRoots,
        bool grouped = false, int page = 1, int size = 5)
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
                CreateTime = DateTime.UtcNow,
                Code = diagnosisIcd10.MkbCode,
                Description = diagnosis.Description,
                Name = diagnosisIcd10.MkbName,
                Type = diagnosis.Type
            };

            await _context.Diagnosis.AddAsync(newDiagnosis);
            await _context.SaveChangesAsync();
            await _context.InspectionDiagnosis.AddAsync(new InspectionDiagnosis
            {
                InspectionId = inspectionId,
                DiagnosisId = newDiagnosis.Id
            });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CreateConsultations(List<ConsultationCreateModel> consultations, Guid doctorId,
        Guid inspectionId)
    {
        foreach (var consultation in consultations)
        {
            var consultationEntity = new Consultation
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.UtcNow,
                InspectionId = inspectionId,
                SpecialityId = consultation.SpecialityId
            };

            await _context.Consultation.AddAsync(consultationEntity);
            await _context.SaveChangesAsync();
            var rootComment = new Comment
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.UtcNow,
                AuthorId = doctorId,
                ConsultationId = consultationEntity.Id,
                Content = consultation.Comment.Content,
                IsRootComment = true
            };

            await _context.Comment.AddAsync(rootComment);
            await _context.SaveChangesAsync();
        }
    }

    private List<PatientModel> CreatePatientModel(List<Patient> patients)
    {
        var patientListModel = new List<PatientModel>();
        foreach (var patient in patients)
        {
            var patientModel = _mapper.Map<PatientModel>(patient);

            patientListModel.Add(patientModel);
        }

        return patientListModel;
    }

    private async Task ValidateInspection(InspectionCreateModel inspectionCreateModel, Guid patientId)
    {
        var previousInspections = await _context.Inspection.Where(i => i.PatientId == patientId).ToListAsync();
        if (previousInspections != null)
        {
            foreach (var inspection in previousInspections)
            {
                if (inspection.Conclusion == Conclusion.Death)
                {
                    throw new BadHttpRequestException("patient dead 💀");
                }
            }
        }

        if (inspectionCreateModel.Date > DateTime.UtcNow)
        {
            throw new BadHttpRequestException("Date mustn't be later then now");
        }

        
        var flag = false;
        foreach (var diagnosis in inspectionCreateModel.Diagnoses)
        {
            if (diagnosis.Type == DiagnosisType.Main)
            {
                if (flag)
                {
                    throw new BadHttpRequestException(
                        "inspection must necessarily have only one diagnosis with the type “Main”");
                }

                flag = true;
            }
        }
        if (!flag)
        {
            throw new BadHttpRequestException("inspection must necessarily have one diagnosis with the type “Main”");
        }

        
        switch (inspectionCreateModel.Conclusion)
        {
            case Conclusion.Disease:
                if (inspectionCreateModel.NextVisitDate == null)
                {
                    throw new BadHttpRequestException(
                        "when conclusion is “Disease”, it is necessary to specify the date and time of the next visit");
                }

                if (inspectionCreateModel.DeathDate != null)
                {
                    throw new BadHttpRequestException("when conclusion is “Disease”, then you can't set a death date");
                }

                break;
            case Conclusion.Death:
                if (inspectionCreateModel.DeathDate == null)
                {
                    throw new BadHttpRequestException(
                        "when conclusion is “Death”, it is necessary to specify the death date");
                }

                if (inspectionCreateModel.NextVisitDate != null)
                {
                    throw new BadHttpRequestException(
                        "when conclusion is “Death”, then you can't set a next visit date");
                }

                break;
            case Conclusion.Recovery:
                if (inspectionCreateModel.NextVisitDate != null)
                {
                    throw new BadHttpRequestException(
                        "when conclusion is “Recovery”, then you can't set a next visit date");
                }

                if (inspectionCreateModel.DeathDate != null)
                {
                    throw new BadHttpRequestException("when conclusion is “Recovery”, then you can't set a death date");
                }

                break;
        }
        
        if (inspectionCreateModel.Consultations != null && inspectionCreateModel.Consultations.Count > 0)
        {
            var uniqueSpecialityIds = new HashSet<Guid>();

            foreach (var consultation in inspectionCreateModel.Consultations)
            {
                if (!uniqueSpecialityIds.Add(consultation.SpecialityId))
                {
                    throw new ArgumentException("duplicate speciality found in consultations");
                }

                if (consultation.Comment == null)
                {
                    throw new BadHttpRequestException("not comment found in consultation");
                }
            }
        }
    }
}