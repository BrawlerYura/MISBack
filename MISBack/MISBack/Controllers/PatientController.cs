using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers;

[ApiController]
[Route("api/patient")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<TokenResponseModel> CreatePatient(PatientCreateModel patientCreateModel)
    {
        return await _patientService.CreatePatient(patientCreateModel);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<PatientPagedListModel> GetPatientsList([FromQuery] string? name,
        [FromQuery] List<string> conclusions, [FromQuery] PatientSorting sorting,
        [FromQuery] bool scheduledVisits = false, [FromQuery] bool onlyMine = false, [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        return await _patientService.GetPatientsList(name, conclusions, sorting, scheduledVisits, onlyMine, page, size);
    }
    
    [HttpPost]
    [Route("{patientId}/inspections")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<InspectionCreateModel> CreateInspectionForPatient(Guid patientId)
    {
        return await _patientService.CreateInspectionForPatient(patientId);
    }
    
    [HttpGet]
    [Route("{patientId}/inspections")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<InspectionPagedListModel> GetPatientsInspectionsList( Guid patientId, 
        [FromQuery] List<string>? icdRoots, [FromQuery] bool grouped = false, [FromQuery] int page = 1,
        [FromQuery] int size = 5)
    {
        return await _patientService.GetPatientsInspectionsList(patientId, icdRoots, grouped, page, size);
    }
    
    [HttpGet]
    [Route("{patientId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<PatientModel> GetPatientCard(Guid patientId)
    {
        return await _patientService.GetPatientCard(patientId);
    }
    
    [HttpGet]
    [Route("{patientId}/inspections/search")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<PatientModel> SearchPatientMedicalInspections(Guid patientId, [FromQuery] string? request)
    {
        return await _patientService.SearchPatientMedicalInspections(patientId, request);
    }
}