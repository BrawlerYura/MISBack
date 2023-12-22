using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
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
    public async Task<DoctorModel> GetDoctorProfile()
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            ?.Value ?? string.Empty);
        
        return await _doctorService.GetDoctorProfile(userId);
    }

    [HttpPut]
    [Route("profile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<DoctorEditModel> EditDoctorProfile()
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            ?.Value ?? string.Empty);
        
        return await _doctorService.EditDoctorProfile(userId);
    }
}