using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IDoctorService
{
    Task<TokenResponseModel> RegisterDoctor(DoctorRegisterModel doctorRegisterModel);

    Task<TokenResponseModel> Login(LoginCredentialsModel credentials);

    Task Logout();

    Task<DoctorModel> GetDoctorProfile();

    Task<DoctorEditModel> EditDoctorProfile();
}