using AutoMapper;
using MISBack.Data.Entities;
using MISBack.Data.Models;

namespace MISBack.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Doctor, DoctorModel>();
        CreateMap<DoctorModel, Doctor>();
        CreateMap<DoctorEditModel, DoctorRegisterModel>();
        CreateMap<Inspection, InspectionPreviewModel>();
        CreateMap<Diagnosis, DiagnosisModel>();
        CreateMap<DiagnosisModel, Diagnosis>();
        CreateMap<Patient, PatientModel>();
        CreateMap<PatientModel, Patient>();
        CreateMap<Consultation, ConsultationModel>();
        CreateMap<Speciality, SpecialityModel>();
        CreateMap<Inspection, InspectionModel>();
        CreateMap<InspectionModel, Inspection>();
        CreateMap<Inspection, InspectionConsultationModel>();
        CreateMap<InspectionConsultationModel, Inspection>();
        CreateMap<InspectionConsultationModel, Consultation>();
        CreateMap<Consultation, InspectionConsultationModel>();
        CreateMap<Comment, InspectionCommentModel>();
        CreateMap<CommentCreateModel, Comment>();
        CreateMap<InspectionCreateModel, Inspection>();
        CreateMap<InspectionShortModel, Inspection>();
        CreateMap<Inspection, InspectionShortModel>();
        CreateMap<Comment, CommentModel>();
        CreateMap<CommentModel, Comment>();
    }
}