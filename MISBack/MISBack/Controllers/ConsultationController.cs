using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers;

[ApiController]
[Route("api/consultation")]
public class ConsultationController : ControllerBase
{
    private readonly IConsultationService _consultationService;

    public ConsultationController(IConsultationService consultationService)
    {
        _consultationService = consultationService;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<InspectionPagedListModel> GetInspectionsList([FromQuery] bool grouped,
        [FromQuery] List<string>? icdRoots, [FromQuery] int page, [FromQuery] int size)
    {
        return await _consultationService.GetInspectionsList(grouped, icdRoots, page, size);
    }
    
    [HttpGet]
    [Route("{consultationId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ConsultationModel> GetConsultation(Guid consultationId)
    {
        return await _consultationService.GetConsultation(consultationId);
    }
    
    [HttpPost]
    [Route("{consultationId}/comment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<Guid> AddComment(Guid consultationId, CommentCreateModel comment)
    {
        var value = User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
        if (value == null) throw new Exception();
        
        var authorId = Guid.Parse(value);
        
        return await _consultationService.AddComment(consultationId, authorId, comment);
    }
    
    [HttpPut]
    [Route("comment/{commentId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task EditComment(Guid commentId, InspectionCommentCreateModel comment)
    {
        await _consultationService.EditComment(commentId, comment);
    }
}