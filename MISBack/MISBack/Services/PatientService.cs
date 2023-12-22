using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class PatientService : IPatientService
{
    public Task<TokenResponseModel> CreatePatient(PatientCreateModel patientCreateModel)
    {
        throw new NotImplementedException();
    }

    public Task<PatientPagedListModel> GetPatientsList(string? name, List<string> conclusions, PatientSorting sorting, bool scheduledVisits = false,
        bool onlyMine = false, int page = 1, int size = 5)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionCreateModel> CreateInspectionForPatient(Guid patientId)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionPagedListModel> GetPatientsInspectionsList(Guid patientId, List<string>? icdRoots, bool grouped = false, int page = 1, int size = 5)
    {
        throw new NotImplementedException();
    }

    public Task<PatientModel> GetPatientCard(Guid patientId)
    {
        throw new NotImplementedException();
    }

    public Task<PatientModel> SearchPatientMedicalInspections(Guid patientId, string? request)
    {
        throw new NotImplementedException();
    }
}