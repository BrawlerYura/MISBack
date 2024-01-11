using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IInspectionService
{
    Task<InspectionModel> GetInspectionInfo(Guid inspectionId);

    Task EditInspection(Guid inspectionId, InspectionEditModel inspectionEditModel);

    Task<List<InspectionPreviewModel>> GetInspectionForRoot(Guid inspectionId);
}