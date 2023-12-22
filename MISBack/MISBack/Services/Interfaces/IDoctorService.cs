using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IDoctorService
{
    Task<TokenResponseModel> RegisterDoctor(DoctorRegisterModel doctorRegisterModel);

    Task<TokenResponseModel> Login(LoginCredentialsModel credentials);

    Task Logout(string token);

    Task<DoctorModel> GetDoctorProfile(Guid userId);

    Task<DoctorEditModel> EditDoctorProfile(Guid userId);
}