using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.AutoMapper;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Migrations;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class InspectionService : IInspectionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public InspectionService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InspectionModel> GetInspectionInfo(Guid inspectionId)
    {
        var inspectionEntity = await _context.Inspection
            .FirstOrDefaultAsync(x => x.Id == inspectionId);
        if (inspectionEntity == null)
        {
            throw new KeyNotFoundException($"inspection with id {inspectionId} not found");
        }

        var inspectionModel = _mapper.Map<InspectionModel>(inspectionEntity);

        inspectionModel.Patient = await MapPatientAsync(inspectionEntity.PatientId);
        inspectionModel.Doctor = await MapDoctorAsync(inspectionEntity.DoctorId);
        inspectionModel.Diagnoses = await MapDiagnosesAsync(inspectionId);
        inspectionModel.Consultations = await MapConsultationsAsync(inspectionId);

        return inspectionModel;
    }

    public async Task EditInspection(Guid inspectionId, InspectionEditModel inspectionEditModel)
    {
        var inspectionEntity = await _context.Inspection
            .FirstOrDefaultAsync(x => x.Id == inspectionId);
        if (inspectionEntity == null)
        {
            throw new KeyNotFoundException($"inspection with id {inspectionId} not found");
        }

        inspectionEntity.Anamnesis = inspectionEditModel.Anamnesis;
        inspectionEntity.Complaints = inspectionEditModel.Complaints;
        inspectionEntity.Treatment = inspectionEditModel.Treatment;
        inspectionEntity.Conclusion = inspectionEditModel.Conclusion;
        inspectionEntity.NextVisitDate = inspectionEditModel.NextVisitDate;
        inspectionEntity.DeathDate = inspectionEditModel.DeathDate;

        await _context.SaveChangesAsync();
        
        foreach (var diagnosis in inspectionEditModel.Diagnoses)
        {
            var diagnosisICD10 =
                await _context.Icd10.Where(d => d.Id == diagnosis.IcdDiagnosisId).FirstOrDefaultAsync();
        
            if (diagnosisICD10 == null)
            {
                throw new KeyNotFoundException($"diagnosis with id {diagnosis.IcdDiagnosisId} not found in icd10");
            }
        
            var newDiagnosis = new Diagnosis
            {
                Id = new Guid(),
                CreateTime = DateTime.UtcNow,
                Code = diagnosisICD10.MkbCode,
                Description = diagnosis.Description,
                Name = diagnosisICD10.MkbName,
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

    public async Task<List<InspectionPreviewModel>> GetInspectionForRoot(Guid rootInspectionId)
    {
        var inspections = await _context.Inspection
            .Where(i => i.BaseInspectionId == rootInspectionId)
            .OrderByDescending(i => i.BaseInspectionId == i.Id)
            .ThenBy(i => i.PreviousInspectionId)
            .ToListAsync();

        if (inspections == null || inspections.Count == 0)
        {
            throw new KeyNotFoundException($"No inspections found for root inspection with id {rootInspectionId}");
        }

        var inspectionModels = new List<InspectionPreviewModel>();

        foreach (var inspection in inspections)
        {
            var inspectionModel = _mapper.Map<InspectionPreviewModel>(inspection);

            var diagnosisEntity = await _context.InspectionDiagnosis
                .Where(d => d.InspectionId == inspection.Id)
                .Join(
                    _context.Diagnosis,
                    id => id.DiagnosisId,
                    d => d.Id,
                    (id, d) => d
                )
                .Where(d => d.Type == DiagnosisType.Main)
                .FirstOrDefaultAsync();

            if (diagnosisEntity != null)
            {
                inspectionModel.Diagnosis = _mapper.Map<DiagnosisModel>(diagnosisEntity);
            }

            var hasNested = await _context.Inspection
                .AnyAsync(i => i.PreviousInspectionId == inspection.Id);

            inspectionModel.HasNested = hasNested;

            inspectionModel.HasChain = inspectionModel.Id == rootInspectionId;
            
            inspectionModels.Add(inspectionModel);
        }

        return inspectionModels;
    }

    private async Task<PatientModel> MapPatientAsync(Guid patientId)
    {
        var patientEntity = await _context.Patient
            .FirstOrDefaultAsync(x => x.Id == patientId);

        if (patientEntity == null)
        {
            throw new KeyNotFoundException($"patient with id {patientId} not found");
        }

        return _mapper.Map<PatientModel>(patientEntity);
    }

    private async Task<DoctorModel> MapDoctorAsync(Guid doctorId)
    {
        var doctorEntity = await _context.Doctor
            .FirstOrDefaultAsync(x => x.Id == doctorId);

        if (doctorEntity == null)
        {
            throw new KeyNotFoundException($"doctor with id {doctorId} not found");
        }

        return _mapper.Map<DoctorModel>(doctorEntity);
    }

    private async Task<List<DiagnosisModel>> MapDiagnosesAsync(Guid inspectionId)
    {
        var diagnosisIdList = await _context.InspectionDiagnosis
            .Where(d => d.InspectionId == inspectionId)
            .Select(d => d.DiagnosisId)
            .ToListAsync();

        var diagnosesList = new List<DiagnosisModel>();

        foreach (var diagnosisId in diagnosisIdList)
        {
            var diagnosisEntity = await _context.Diagnosis
                .FirstOrDefaultAsync(d => d.Id == diagnosisId);

            if (diagnosisEntity == null)
            {
                throw new KeyNotFoundException($"diagnosis with id {diagnosisId} not found");
            }

            var diagnosisModel = _mapper.Map<DiagnosisModel>(diagnosisEntity);
            diagnosesList.Add(diagnosisModel);
        }

        return diagnosesList;
    }

    private async Task<List<InspectionConsultationModel>> MapConsultationsAsync(Guid inspectionId)
    {
        var consultations = await _context.Consultation
            .Where(c => c.InspectionId == inspectionId)
            .ToListAsync();

        var consultationsModelList = new List<InspectionConsultationModel>();
        foreach (var consultation in consultations)
        {
            var consultationModel = _mapper.Map<InspectionConsultationModel>(consultation);
            var commentList = await _context.Comment.Where(c => c.ConsultationId == consultation.Id).ToListAsync();
            if (commentList == null)
            {
                throw new KeyNotFoundException($"comments not found");
            }

            consultationModel.CommentsNumber = commentList.Count;
            var rootComment = await _context.Comment.Where(c => c.ConsultationId == consultation.Id && c.IsRootComment)
                .FirstOrDefaultAsync();
            if (rootComment == null)
            {
                throw new KeyNotFoundException($"root comment not found");
            }

            consultationModel.RootComment = _mapper.Map<InspectionCommentModel>(rootComment);
            var author = await _context.Doctor.Where(d => d.Id == rootComment.AuthorId).FirstOrDefaultAsync();
            if (author == null)
            {
                throw new KeyNotFoundException($"author with id {rootComment.AuthorId} comment not found");
            }

            consultationModel.RootComment.Author = _mapper.Map<DoctorModel>(author);

            consultationsModelList.Add(consultationModel);
        }

        return consultationsModelList;
    }
}