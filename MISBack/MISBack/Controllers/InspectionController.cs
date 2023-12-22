using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Controllers;

[ApiController]
[Route("api/inspection")]
public class InspectionController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public InspectionController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }
    
    [HttpGet]
    [Route("{inspectionId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<InspectionModel> GetInspectionInfo(Guid inspectionId)
    {
        return await _doctorService.GetInspectionInfo(inspectionId);
    }
    
    [HttpPut]
    [Route("{inspectionId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<InspectionModel> EditInspection(Guid inspectionId, InspectionEditModel inspectionEditModel)
    {
        return await _doctorService.EditInspection(inspectionId, inspectionEditModel);
    }
    
    [HttpGet]
    [Route("{inspectionId}/chain")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<InspectionModel> GetInspectionForRoot(Guid inspectionId)
    {
        return await _doctorService.GetInspectionForRoot(inspectionId);
    }
}