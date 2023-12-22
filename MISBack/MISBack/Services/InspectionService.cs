using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class InspectionService : IInspectionService
{
    public Task<InspectionModel> GetInspectionInfo(Guid inspectionId)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionModel> EditInspection(Guid inspectionId, InspectionEditModel inspectionEditModel)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionModel> GetInspectionForRoot(Guid inspectionId)
    {
        throw new NotImplementedException();
    }
}