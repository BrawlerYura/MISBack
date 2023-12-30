using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
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
        
        var patientEntity = await _context.Patient
            .FirstOrDefaultAsync(x => x.Id == inspectionEntity.PatientId);

        if (patientEntity != null)
        {
            var patient = _mapper.Map<PatientModel>(patientEntity);
            inspectionModel.Patient = patient;    
        }

        var doctorEntity = await _context.Doctor
            .FirstOrDefaultAsync(x => x.Id == inspectionEntity.DoctorId);

        if (doctorEntity != null)
        {
            var doctor = _mapper.Map<DoctorModel>(doctorEntity);
            inspectionModel.Doctor = doctor;    
        }

        var diagnosesList = await _context.Diagnosis.Where(d => d.InspectionId == inspectionId).ToListAsync();

        var diagnosesModelList = _mapper.Map<List<DiagnosisModel>>(diagnosesList);
        inspectionModel.Diagnoses = diagnosesModelList;

        var consultations = await _context.Consultation.Where(c => c.InspectionId == inspectionId).ToListAsync();
        
        var consultationsModelList = _mapper.Map<List<InspectionConsultationModel>>(consultations);
        inspectionModel.Consultations = consultationsModelList;

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

        inspectionEntity.Anamnesis =  inspectionEditModel.Anamnesis;
        inspectionEntity.Complaints = inspectionEditModel.Complaints;
        inspectionEntity.Treatment =  inspectionEditModel.Treatment;
        inspectionEntity.Conclusion = inspectionEditModel.Conclusion;
        inspectionEntity.NextVisitDate = inspectionEditModel.NextVisitDate;
        inspectionEntity.DeathDate = inspectionEditModel.DeathDate;

        await _context.SaveChangesAsync();
        
        foreach (var diagnosis in inspectionEditModel.Diagnoses)
        {
            await _context.Diagnosis.AddAsync(new Diagnosis
                {
                    Id = diagnosis.IcdDiagnosisId
                }
            );
            await _context.SaveChangesAsync();
        }
    }

    public Task<InspectionModel> GetInspectionForRoot(Guid inspectionId)
    {
        throw new NotImplementedException();
    }
}