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
        CreateMap<Inspection, InspectionPreviewModel>();
        CreateMap<Diagnosis, DiagnosisModel>();
        CreateMap<Patient, PatientModel>();
        CreateMap<Consultation, ConsultationModel>();
        CreateMap<Speciality, SpecialityModel>();
        CreateMap<Inspection, InspectionModel>();
        CreateMap<Inspection, InspectionConsultationModel>();
        CreateMap<Comment, InspectionCommentModel>();
        CreateMap<CommentCreateModel, Comment>();
        CreateMap<InspectionCreateModel, Inspection>();
    }
}