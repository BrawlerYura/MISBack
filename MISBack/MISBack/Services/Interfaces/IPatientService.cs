using MISBack.Data.Enums;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IPatientService
{
    Task<Guid> CreatePatient(PatientCreateModel patientCreateModel);

    Task<PatientPagedListModel> GetPatientsList(Guid doctorId, string? name, List<Conclusion>? conclusions, PatientSorting sorting,
        bool scheduledVisits = false, bool onlyMine = false, int page = 1, int size = 5);

    Task<Guid> CreateInspectionForPatient(Guid patientId, Guid doctorId, InspectionCreateModel inspectionCreateModel);

    Task<InspectionPagedListModel> GetPatientsInspectionsList(Guid patientId, List<string>? icdRoots,
        bool grouped = false, int page = 1, int size = 5);

    Task<PatientModel> GetPatientCard(Guid patientId);

    Task<List<InspectionShortModel>> SearchPatientMedicalInspections(Guid patientId, string? request);
}