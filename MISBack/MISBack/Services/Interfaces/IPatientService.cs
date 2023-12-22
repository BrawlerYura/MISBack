using MISBack.Data.Enums;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IPatientService
{
    Task<TokenResponseModel> CreatePatient(PatientCreateModel patientCreateModel);

    Task<PatientPagedListModel> GetPatientsList(string? name, List<string> conclusions, PatientSorting sorting,
        bool scheduledVisits = false, bool onlyMine = false, int page = 1, int size = 5);

    Task<InspectionCreateModel> CreateInspectionForPatient(Guid patientId);

    Task<InspectionPagedListModel> GetPatientsInspectionsList(Guid patientId, List<string>? icdRoots,
        bool grouped = false, int page = 1, int size = 5);

    Task<PatientModel> GetPatientCard(Guid patientId);

    Task<PatientModel> SearchPatientMedicalInspections(Guid patientId, string? request);
}