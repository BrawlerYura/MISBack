using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces;

public interface IConsultationService
{
    Task<InspectionPagedListModel> GetInspectionsList([FromQuery] List<string>? icdRoots, [FromQuery] int page,
        [FromQuery] int size, [FromQuery] bool grouped);

    Task<ConsultationModel> GetConsultation(Guid consultationId);

    Task<Guid> AddComment(Guid consultationId, Guid authorId, CommentCreateModel comment);

    Task EditComment(Guid authorId, Guid commentId, InspectionCommentCreateModel comment);
}