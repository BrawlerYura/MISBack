using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IConsultationService
{
    Task<InspectionPagedListModel> GetInspectionsList([FromQuery] bool grouped,
        [FromQuery] List<string> icdRoots, [FromQuery] int page, [FromQuery] int size);

    Task<ConsultationModel> GetConsultation(Guid consultationId);

    Task<Guid> AddComment(Guid consultationId, CommentCreateModel comment);

    Task EditComment(Guid commentId, InspectionCommentCreateModel comment);
}