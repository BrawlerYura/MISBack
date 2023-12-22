using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers;

[ApiController]
[Route("api/doctor")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<TokenResponseModel> RegisterDoctor(DoctorRegisterModel doctorRegisterModel)
    {
        return await _doctorService.RegisterDoctor(doctorRegisterModel);
    }

    [HttpPost]
    [Route("login")]
    public async Task<TokenResponseModel> Login(LoginCredentialsModel credentials)
    {
        return await _doctorService.Login(credentials);
    }

    [HttpPost]
    [Route("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "ValidateToken")]
    public async Task Logout()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
        {
            throw new Exception("Token not found");
        }

        await _doctorService.Logout(token);
    }

    [HttpGet]
    [Route("profile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "ValidateToken")]
    public async Task<DoctorModel> GetDoctorProfile()
    {
        var value = User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
        if (value == null) throw new Exception();
        
        var userId = Guid.Parse(value);
        
        return await _doctorService.GetDoctorProfile(userId);
    }

    [HttpPut]
    [Route("profile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "ValidateToken")]
    public async Task EditDoctorProfile(DoctorEditModel doctorEditModel)
    {
        var value = User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
        if (value == null) throw new Exception();
        
        var userId = Guid.Parse(value);
        await _doctorService.EditDoctorProfile(userId, doctorEditModel);
    }
}