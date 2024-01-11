using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers;

[ApiController]
[Route("api/inspection")]
public class InspectionController : ControllerBase
{
    private readonly IInspectionService _inspectionService;

    public InspectionController(IInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }
    
    [HttpGet]
    [Route("{inspectionId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<InspectionModel> GetInspectionInfo(Guid inspectionId)
    {
        return await _inspectionService.GetInspectionInfo(inspectionId);
    }
    
    [HttpPut]
    [Route("{inspectionId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task EditInspection(Guid inspectionId, InspectionEditModel inspectionEditModel)
    {
        await _inspectionService.EditInspection(inspectionId, inspectionEditModel);
    }
    
    [HttpGet]
    [Route("{inspectionId}/chain")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<List<InspectionPreviewModel>> GetInspectionForRoot(Guid inspectionId)
    {
        return await _inspectionService.GetInspectionForRoot(inspectionId);
    }
}