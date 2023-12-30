using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
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

    public Task<InspectionModel> EditInspection(Guid inspectionId, InspectionEditModel inspectionEditModel)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionModel> GetInspectionForRoot(Guid inspectionId)
    {
        throw new NotImplementedException();
    }
}