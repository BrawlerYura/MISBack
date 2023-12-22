using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class ConsultationService : IConsultationService
{
    public Task<InspectionPagedListModel> GetInspectionsList(bool grouped, List<string> icdRoots, int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<ConsultationModel> GetConsultation(Guid consultationId)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> AddComment(Guid consultationId, CommentCreateModel comment)
    {
        throw new NotImplementedException();
    }

    public Task EditComment(Guid commentId, InspectionCommentCreateModel comment)
    {
        throw new NotImplementedException();
    }
}