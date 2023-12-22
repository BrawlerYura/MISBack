using AutoMapper;
using MISBack.Data.Entities;
using MISBack.Data.Models;

namespace MISBack.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Doctor, DoctorModel>();
        CreateMap<DoctorEditModel, DoctorRegisterModel>();
    }
}