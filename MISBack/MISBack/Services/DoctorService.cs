using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class DoctorService : IDoctorService
{
    public Task<TokenResponseModel> RegisterDoctor(DoctorRegisterModel doctorRegisterModel)
    {
        throw new NotImplementedException();
    }

    public Task<TokenResponseModel> Login(LoginCredentialsModel credentials)
    {
        throw new NotImplementedException();
    }

    public Task Logout()
    {
        throw new NotImplementedException();
    }

    public Task<DoctorModel> GetDoctorProfile()
    {
        throw new NotImplementedException();
    }

    public Task<DoctorEditModel> EditDoctorProfile()
    {
        throw new NotImplementedException();
    }
}