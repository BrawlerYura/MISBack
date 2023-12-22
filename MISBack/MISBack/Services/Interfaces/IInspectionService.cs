using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IInspectionService
{
    Task<InspectionModel> GetInspectionInfo(Guid inspectionId);

    Task<InspectionModel> EditInspection(Guid inspectionId, InspectionEditModel inspectionEditModel);

    Task<InspectionModel> GetInspectionForRoot(Guid inspectionId);
}